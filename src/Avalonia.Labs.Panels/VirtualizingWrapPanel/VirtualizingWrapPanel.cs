using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls.Utils;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Controls;

/// <summary>
/// A implementation of a wrap panel that supports virtualization and can be used in horizontal and vertical orientation.
/// </summary>
public class VirtualizingWrapPanel : VirtualizingPanel, IScrollSnapPointsInfo, IItemSizeProvider
{
    // The fallback size in case size calculation went wrong
    private static readonly Size _FallbackItemSize = new Size(48, 48);
    private Size? _sizeOfFirstItem;
    private Size? _averageItemSizeCache;

    private int _startItemIndex = -1;
    private int _endItemIndex = -1;

    private double _startItemOffsetX;
    private double _startItemOffsetY;

    private double _knownExtentX;
        
    /// <summary>
    /// Gets an empty size
    /// </summary>
    private static readonly Size _EmptySize = new Size(0, 0);

    private const double EPSILON = 0.001;
        
    static VirtualizingWrapPanel()
    {
        AffectsMeasure<VirtualizingWrapPanel>(
            OrientationProperty,
            ItemSizeProperty,
            AllowDifferentSizedItemsProperty,
            ItemSizeProviderProperty);

        AffectsArrange<VirtualizingWrapPanel>(
            SpacingModeProperty,
            StretchItemsProperty,
            IsGridLayoutEnabledProperty);
    }

    /// <summary>
    /// Defines the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        WrapPanel.OrientationProperty.AddOwner<VirtualizingWrapPanel>(
            new StyledPropertyMetadata<Orientation>(Orientation.Horizontal));

    /// <summary>
    /// Defines the <see cref="ItemSize"/> property.
    /// </summary>
    public static readonly StyledProperty<Size> ItemSizeProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, Size>(nameof(ItemSize), _EmptySize);

    /// <summary>
    /// Defines the <see cref="AllowDifferentSizedItems"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowDifferentSizedItemsProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(nameof(AllowDifferentSizedItems));

    /// <summary>
    /// Defines the <see cref="ItemSizeProvider"/> property.
    /// </summary>
    public static readonly StyledProperty<IItemSizeProvider?> ItemSizeProviderProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, IItemSizeProvider?>(nameof(ItemSizeProvider));

    /// <summary>
    /// /// Defines the <see cref="SpacingMode"/> property.
    /// </summary>
    public static readonly StyledProperty<SpacingMode> SpacingModeProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, SpacingMode>(nameof(SpacingMode), SpacingMode.Uniform);

    /// <summary>
    /// Defines the <see cref="StretchItems"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> StretchItemsProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(nameof(StretchItems));

    /// <summary>
    /// Defines the <see cref="IsGridLayoutEnabled"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsGridLayoutEnabledProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(nameof(IsGridLayoutEnabled), true);


    /// <summary>
    /// Defines the <see cref="CacheRows"/> property.
    /// </summary>
    public static readonly StyledProperty<int> CacheRowsProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, int>(nameof(CacheRows), 2);

    /// <summary>
    /// Defines the RecycleKey attached property.
    /// </summary>
    private static readonly AttachedProperty<object?> _RecycleKeyProperty =
        AvaloniaProperty.RegisterAttached<VirtualizingWrapPanel, Control, object?>("RecycleKey");
        
    private static readonly object s_itemIsItsOwnContainer = new object();
    private readonly Action<Control, int> _recycleElement;
    private readonly Action<Control> _recycleElementOnItemRemoved;
    private readonly Action<Control, int, int> _updateElementIndex;
    private int _scrollToIndex = -1;
    private Control? _scrollToElement;
    private bool _isInLayout;
    private bool _isWaitingForViewportUpdate;
    private RealizedWrapElements? _measureElements;
    private RealizedWrapElements? _realizedElements;
    private IScrollAnchorProvider? _scrollAnchorProvider;
    private Rect _viewport;
    private Dictionary<object, Stack<Control>>? _recyclePool;
    private Control? _focusedElement;
        
    /// <summary>
    /// Stores information about a row of items.
    /// </summary>
    private sealed class RowInfo
    {
        public int StartIndex;
        public double Y;
        public double Height;
        public int Count;
        public double SummedUpChildWidth;
    }

    private readonly List<RowInfo> _rowCache = new();
    private const int RowCacheCapacity = 256; // "some rows" to improve performance without excessive memory
    private double _lastLayoutWidth;
    private int _focusedIndex = -1;
    private double? _navigationAnchor;
    private int _lastNavigationIndex = -1;

    private void ClearRowCache()
    {
        _rowCache.Clear();
    }

    private void AddRowCacheEntry(int startIndex, double y, double height, int count, double summedUpChildWidth)
    {
        if (count <= 0)
            return;

        int index = -1;
        // Optimization: check last entry first as it's the most common case during forward realization
        if (_rowCache.Count > 0)
        {
            var last = _rowCache[_rowCache.Count - 1];
            if (last.StartIndex == startIndex)
            {
                index = _rowCache.Count - 1;
            }
            else if (last.StartIndex < startIndex)
            {
                // New entry after the last one, will be handled by the Add at the end
            }
            else
            {
                // Out of order or scrolling up, find insertion point or existing entry using binary search
                int lo = 0, hi = _rowCache.Count - 1;
                while (lo <= hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (_rowCache[mid].StartIndex == startIndex)
                    {
                        index = mid;
                        break;
                    }
                    if (_rowCache[mid].StartIndex < startIndex)
                    {
                        lo = mid + 1;
                    }
                    else
                    {
                        hi = mid - 1;
                    }
                }

                if (index < 0)
                {
                    // Insertion point is at 'lo'
                    index = lo;
                    _rowCache.Insert(index, new RowInfo { StartIndex = startIndex, Y = y, Height = height, Count = count, SummedUpChildWidth = summedUpChildWidth });
                    if (index + 1 < _rowCache.Count)
                    {
                        _rowCache.RemoveRange(index + 1, _rowCache.Count - (index + 1));
                    }
                    goto Trim;
                }
            }
        }

        if (index >= 0)
        {
            var existing = _rowCache[index];
            // If the row info changed, we must invalidate all subsequent rows in the cache
            // as their Y position depends on this row.
            if (!existing.Y.IsCloseTo(y) || !existing.Height.IsCloseTo(height) || existing.Count != count || !existing.SummedUpChildWidth.IsCloseTo(summedUpChildWidth))
            {
                if (index + 1 < _rowCache.Count)
                {
                    _rowCache.RemoveRange(index + 1, _rowCache.Count - (index + 1));
                }
                existing.Y = y;
                existing.Height = height;
                existing.Count = count;
                existing.SummedUpChildWidth = summedUpChildWidth;
            }
        }
        else
        {
            _rowCache.Add(new RowInfo { StartIndex = startIndex, Y = y, Height = height, Count = count, SummedUpChildWidth = summedUpChildWidth });
            index = _rowCache.Count - 1;
        }

    Trim:
        if (_rowCache.Count > RowCacheCapacity)
        {
            // If we are adding/updating near the start, trim from the end.
            // Otherwise trim from the start.
            if (index < _rowCache.Count / 2)
            {
                _rowCache.RemoveAt(_rowCache.Count - 1);
            }
            else
            {
                _rowCache.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualizingWrapPanel"/> class.
    /// </summary>
    public VirtualizingWrapPanel()
    {
        _recycleElement = RecycleElement;
        _recycleElementOnItemRemoved = RecycleElementOnItemRemoved;
        _updateElementIndex = UpdateElementIndex;
        EffectiveViewportChanged += OnEffectiveViewportChanged;
    }

    /// <summary>
    /// Gets or sets a value that specifies the orientation in which items are arranged before wrapping.
    /// The default value is <see cref="Orientation.Horizontal"/>.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies the size of the items. The default value is <see cref="_EmptySize"/>.
    /// If the value is <see cref="_EmptySize"/> the item size is determined by measuring the first realized item.
    /// </summary>
    public Size ItemSize
    {
        get => GetValue(ItemSizeProperty);
        set => SetValue(ItemSizeProperty, value);
    }

    /// <summary>
    /// Specifies whether items can have different sizes. The default value is false. If this property is enabled,
    /// it is strongly recommended to also set the <see cref="ItemSizeProvider"/> property. Otherwise, the position
    /// of the items is not always guaranteed to be correct.
    /// </summary>
    public bool AllowDifferentSizedItems
    {
        get => GetValue(AllowDifferentSizedItemsProperty);
        set => SetValue(AllowDifferentSizedItemsProperty, value);
    }

    /// <summary>
    /// Specifies an instance of <see cref="IItemSizeProvider"/> which provides the size of the items. In order to allow
    /// different sized items, also enable the <see cref="AllowDifferentSizedItems"/> property.
    /// </summary>
    public IItemSizeProvider? ItemSizeProvider
    {
        get => GetValue(ItemSizeProviderProperty);
        set => SetValue(ItemSizeProviderProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing mode used when arranging the items. The default value is <see cref="SpacingMode.Uniform"/>.
    /// </summary>
    public SpacingMode SpacingMode
    {
        get => GetValue(SpacingModeProperty);
        set => SetValue(SpacingModeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies if the items get stretched to fill up remaining space. The default value is false.
    /// </summary>
    /// <remarks>
    /// The MaxWidth and MaxHeight properties of the ItemContainerStyle can be used to limit the stretching.
    /// In this case the use of the remaining space will be determined by the SpacingMode property.
    /// </remarks>
    public bool StretchItems
    {
        get => GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    /// <summary>
    /// Specifies whether the items are arranged in a grid-like layout. The default value is <c>true</c>.
    /// When set to <c>true</c>, the items are arranged based on the number of items that can fit in a row.
    /// When set to <c>false</c>, the items are arranged based on the number of items that are actually placed in the row.
    /// </summary>
    /// <remarks>
    /// If <see cref="AllowDifferentSizedItems"/> is enabled, this property has no effect and the items are always
    /// arranged based on the number of items that are actually placed in the row.
    /// </remarks>
    public bool IsGridLayoutEnabled
    {
        get => GetValue(IsGridLayoutEnabledProperty);
        set => SetValue(IsGridLayoutEnabledProperty, value);
    }

    /// <summary>
    /// Number of rows to keep cached above and below the visible viewport.
    /// Increasing this can reduce layout recalculations while scrolling at the cost of extra realized work.
    /// </summary>
    public int CacheRows
    {
        get => GetValue(CacheRowsProperty);
        set => SetValue(CacheRowsProperty, value);
    }

    /// <summary>
    /// Gets the index of the first realized element, or -1 if no elements are realized.
    /// </summary>
    public int FirstRealizedIndex => _realizedElements?.FirstIndex ?? -1;

    /// <summary>
    /// Gets the index of the last realized element, or -1 if no elements are realized.
    /// </summary>
    public int LastRealizedIndex => _realizedElements?.LastIndex ?? -1;
        

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var items = Items;

        if (items.Count == 0)
            return default;

        var orientation = Orientation;

        var wrappingWidth = GetWidth(availableSize);
        if (double.IsInfinity(wrappingWidth))
        {
            wrappingWidth = GetWidth(_viewport.Size);
        }

        if (wrappingWidth <= 0) wrappingWidth = _lastLayoutWidth;
        if (wrappingWidth <= 0) wrappingWidth = GetWidth(Bounds.Size);
        if (wrappingWidth <= 0) wrappingWidth = GetWidth(DesiredSize);
        if (wrappingWidth <= 0) wrappingWidth = _FallbackItemSize.Width * 10;

        // If we're bringing an item into view, ignore any layout passes until we receive a new
        // effective viewport.
        if (_isWaitingForViewportUpdate)
            return EstimateDesiredSize(orientation, items.Count, wrappingWidth);

        _isInLayout = true;

        try
        {
            // _realizedElements?.ValidateStartU(Orientation);
            _realizedElements ??= new();
            _measureElements ??= new();

            // If the viewport is disjunct then we can recycle everything
            var disjunct = _startItemIndex < _realizedElements.FirstIndex
                           || _startItemIndex > _realizedElements.LastIndex;

            if (disjunct)
                _realizedElements.RecycleAllElements(_recycleElement);

            // Do the measure, creating/recycling elements as necessary to fill the viewport. Don't
            // write to _realizedElements yet, only _measureElements.
            RealizeAndVirtualizeItems();

            // Now swap the measureElements and realizedElements collection.
            (_measureElements, _realizedElements) = (_realizedElements, _measureElements);
            _measureElements.ResetForReuse();

            // If there is a focused element is outside the visible viewport (i.e.
            // _focusedElement is non-null), ensure it's measured.
            _focusedElement?.Measure(availableSize);

            return CalculateDesiredSize(orientation, items.Count, wrappingWidth);
        }
        finally
        {
            _isInLayout = false;
        }
    }

    private readonly List<Control> _rowChildrenReuse = new();
    private readonly List<Size> _childSizesReuse = new();

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_realizedElements is null)
            return default;

        _isInLayout = true;

        try
        {
            if (_startItemIndex == -1)
            {
                return finalSize;
            }

            if (_realizedElements.Count < _endItemIndex - _startItemIndex + 1)
            {
                return finalSize;
            }

            double x = _startItemOffsetX; // + GetX(_viewport.TopLeft);
            double y = _startItemOffsetY; // - GetY(_viewport.TopLeft);
            double rowHeight = 0;
            double maxRowHeight;
            var finalWidth = GetWidth(finalSize);
            var items = Items;

            _rowChildrenReuse.Clear();
            _childSizesReuse.Clear();
            double summedUpChildWidth = 0;

            for (int i = _startItemIndex; i <= _endItemIndex; i++)
            {
                var item = items[i];
                var child = _realizedElements.GetElement(i);

                Size? upfrontKnownItemSize = GetUpfrontKnownItemSize(item);

                Size childSize = upfrontKnownItemSize ??
                                 _realizedElements.GetElementSize(child) ?? _FallbackItemSize;

                if (_rowChildrenReuse.Count > 0 && x + GetWidth(childSize) > finalWidth)
                {
                    ArrangeRow(finalWidth, _rowChildrenReuse, _childSizesReuse, y, summedUpChildWidth);
                    x = 0;
                    // Calculate max height directly instead of using LINQ
                    if (AllowDifferentSizedItems)
                    {
                        maxRowHeight = 0;
                        for (int j = 0; j < _childSizesReuse.Count; j++)
                        {
                            var height = GetHeight(_childSizesReuse[j]);
                            if (height > maxRowHeight)
                                maxRowHeight = height;
                        }
                    }
                    else
                    {
                        maxRowHeight = GetHeight(_childSizesReuse[0]);
                    }
                    y += maxRowHeight;
                    rowHeight = 0;
                    _rowChildrenReuse.Clear();
                    _childSizesReuse.Clear();
                    summedUpChildWidth = 0;
                }

                x += GetWidth(childSize);
                rowHeight = Math.Max(rowHeight, GetHeight(childSize));
                if (child != null)
                {
                    _rowChildrenReuse.Add(child);
                    _childSizesReuse.Add(childSize);
                    summedUpChildWidth += GetWidth(childSize);

                    _scrollAnchorProvider?.RegisterAnchorCandidate(child);
                }
            }

            if (_rowChildrenReuse.Count > 0)
            {
                ArrangeRow(finalWidth, _rowChildrenReuse, _childSizesReuse, y, summedUpChildWidth);
            }

            // Ensure that the focused element is in the correct position.
            if (_focusedElement is not null && _focusedIndex >= 0)
            {
                var startPoint = FindItemOffset(_focusedIndex, finalWidth);

                double focusedOffsetX = GetX(startPoint);
                double focusedOffsetY = GetY(startPoint);

                var rect = Orientation == Orientation.Horizontal ?
                    new Rect(focusedOffsetX, focusedOffsetY, _focusedElement.DesiredSize.Width, _focusedElement.DesiredSize.Height) :
                    new Rect(focusedOffsetY, focusedOffsetX, _focusedElement.DesiredSize.Width, _focusedElement.DesiredSize.Height);
                _focusedElement.Arrange(rect);
            }

            // Ensure that the scrollTo element is in the correct position.
            if (_scrollToElement is not null && _scrollToIndex >= 0)
            {
                var startPoint = FindItemOffset(_scrollToIndex, finalWidth);

                double scrollToOffsetX = GetX(startPoint);
                double scrollToOffsetY = GetY(startPoint);

                var rect = Orientation == Orientation.Horizontal ?
                    new Rect(scrollToOffsetX, scrollToOffsetY, _scrollToElement.DesiredSize.Width,
                        finalSize.Height) :
                    new Rect(scrollToOffsetY, scrollToOffsetX, finalSize.Width,
                        _scrollToElement.DesiredSize.Height);
                _scrollToElement.Arrange(rect);
            }

            return finalSize;
        }
        finally
        {
            _isInLayout = false;

            RaiseEvent(new RoutedEventArgs(Orientation == Orientation.Horizontal ?
                HorizontalSnapPointsChangedEvent :
                VerticalSnapPointsChangedEvent));
        }
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scrollAnchorProvider = this.FindAncestorOfType<IScrollAnchorProvider>();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scrollAnchorProvider = null;
    }

    /// <inheritdoc />
    protected override void OnItemsChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs e)
    {
        _averageItemSizeCache = null;
        ClearRowCache();
        InvalidateMeasure();

        if (_realizedElements is null)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                _realizedElements.ItemsInserted(e.NewStartingIndex, e.NewItems!.Count, _updateElementIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                _realizedElements.ItemsRemoved(e.OldStartingIndex, e.OldItems!.Count, _updateElementIndex,
                    _recycleElementOnItemRemoved);
                break;
            case NotifyCollectionChangedAction.Replace:
                _realizedElements.ItemsReplaced(e.OldStartingIndex, e.OldItems!.Count,
                    _recycleElementOnItemRemoved);
                break;
            case NotifyCollectionChangedAction.Move:
                _realizedElements.ItemsRemoved(e.OldStartingIndex, e.OldItems!.Count, _updateElementIndex,
                    _recycleElementOnItemRemoved);
                _realizedElements.ItemsInserted(e.NewStartingIndex, e.NewItems!.Count, _updateElementIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                _realizedElements.ItemsReset(_recycleElementOnItemRemoved);
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnItemsControlChanged(ItemsControl? oldValue)
    {
        base.OnItemsControlChanged(oldValue);

        if (oldValue is not null)
            oldValue.PropertyChanged -= OnItemsControlPropertyChanged;
        if (ItemsControl is not null)
            ItemsControl.PropertyChanged += OnItemsControlPropertyChanged;
    }

    /// <inheritdoc />
    protected override IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
    {
        var count = Items.Count;
        var fromControl = from as Control;

        if (count == 0 ||
            (fromControl is null && direction is not NavigationDirection.First and not NavigationDirection.Last))
            return null;

        var fromIndex = fromControl != null ? IndexFromContainer(fromControl) : -1;
        var toIndex = fromIndex;

        if (fromIndex != _lastNavigationIndex)
        {
            _navigationAnchor = null;
        }

        // Reset or update navigation anchor
        switch (direction)
        {
            case NavigationDirection.Up:
            case NavigationDirection.Down:
                if (Orientation == Orientation.Vertical)
                    _navigationAnchor = null;
                break;
            case NavigationDirection.Left:
            case NavigationDirection.Right:
                if (Orientation == Orientation.Horizontal)
                    _navigationAnchor = null;
                break;
            default:
                _navigationAnchor = null;
                break;
        }

        switch (direction)
        {
            case NavigationDirection.First:
                toIndex = 0;
                break;
            case NavigationDirection.Last:
                toIndex = count - 1;
                break;
            case NavigationDirection.Next:
                NavigateRight(ref toIndex);
                break;
            case NavigationDirection.Previous:
                NavigateLeft(ref toIndex);
                break;
            case NavigationDirection.Left:
                NavigateLeft(ref toIndex);
                break;
            case NavigationDirection.Right:
                NavigateRight(ref toIndex);
                break;
            case NavigationDirection.Up:
                NavigateUp(ref toIndex);
                break;
            case NavigationDirection.Down:
                NavigateDown(ref toIndex);
                break;
            default:
                return null;
        }

        if (fromIndex == toIndex)
        {
            _lastNavigationIndex = toIndex;
            return from;
        }

        if (wrap)
        {
            if (toIndex < 0)
                toIndex = count - 1;
            else if (toIndex >= count)
                toIndex = 0;
        }
        else
        {
            if (toIndex < 0)
                toIndex = 0;
            else if (toIndex >= count)
                toIndex = count - 1;
        }

        _lastNavigationIndex = toIndex;
        return ScrollIntoView(toIndex);
    }

    /// <inheritdoc />
    protected override IEnumerable<Control>? GetRealizedContainers()
    {
        if (_realizedElements is null)
        {
            return null;
        }

        var elements = _realizedElements.Elements;
        var count = elements.Count;
        var result = new List<Control>(count);
        for (var i = 0; i < count; i++)
        {
            if (elements[i] is { } element)
            {
                result.Add(element);
            }
        }

        return result;
    }

    /// <inheritdoc />
    protected override Control? ContainerFromIndex(int index)
    {
        if (index < 0 || index >= Items.Count)
            return null;
        if (_scrollToIndex == index)
            return _scrollToElement;
        if (_focusedIndex == index)
            return _focusedElement;
        if (GetRealizedElement(index) is { } realized)
            return realized;
        if (Items[index] is Control c && c.GetValue(_RecycleKeyProperty) == s_itemIsItsOwnContainer)
            return c;
        return null;
    }

    /// <inheritdoc />
    protected override int IndexFromContainer(Control container)
    {
        if (container == _scrollToElement)
            return _scrollToIndex;
        if (container == _focusedElement)
            return _focusedIndex;
        return _realizedElements?.GetIndex(container) ?? -1;
    }

    /// <inheritdoc />
    protected override Control? ScrollIntoView(int index)
    {
        var items = Items;

        if (_isInLayout || index < 0 || index >= items.Count || _realizedElements is null || !IsEffectivelyVisible)
            return null;

        var wrappingWidth = GetWrappingWidth();

        if (GetRealizedElement(index) is { } element)
        {
            if (this.GetVisualRoot() is ILayoutRoot root)
            {
                var start = FindItemOffset(index, wrappingWidth);
                var rect = new Rect(GetX(start), GetY(start), GetWidth(element.DesiredSize),
                    GetHeight(element.DesiredSize));

                if (!_viewport.Contains(rect))
                {
                    _isWaitingForViewportUpdate = true;
                    (root as Layoutable)?.UpdateLayout();
                    _isWaitingForViewportUpdate = false;
                }
            }

            element.BringIntoView();
            return element;
        }
        else if (this.GetVisualRoot() is ILayoutRoot root)
        {
            // Create and measure the element to be brought into view. Store it in a field so that
            // it can be re-used in the layout pass.
            var scrollToElement = GetOrCreateElement(items, index);
            scrollToElement.Measure(Size.Infinity);

            // Get the expected position of the element and put it in place.
            var start = FindItemOffset(index, wrappingWidth);
            var rect = new Rect(GetX(start), GetY(start), GetWidth(scrollToElement.DesiredSize),
                GetHeight(scrollToElement.DesiredSize));
            scrollToElement.Arrange(rect);

            // Store the element and index so that they can be used in the layout pass.
            _scrollToElement = scrollToElement;
            _scrollToIndex = index;

            // If the item being brought into view was added since the last layout pass then
            // our bounds won't be updated, so any containing scroll viewers will not have an
            // updated extent. Do a layout pass to ensure that the containing scroll viewers
            // will be able to scroll the new item into view.
            if (!Bounds.Contains(rect) && !_viewport.Contains(rect))
            {
                _isWaitingForViewportUpdate = true;
                (root as Layoutable)?.UpdateLayout();
                _isWaitingForViewportUpdate = false;
            }

            // Try to bring the item into view.
            scrollToElement.BringIntoView();

            // If the viewport does not contain the item to scroll to, set _isWaitingForViewportUpdate:
            // this should cause the following chain of events:
            // - Measure is first done with the old viewport (which will be a no-op, see MeasureOverride)
            // - The viewport is then updated by the layout system which invalidates our measure
            // - Measure is then done with the new viewport.
            _isWaitingForViewportUpdate = !_viewport.Contains(rect);
            (root as Layoutable)?.UpdateLayout();

            // If for some reason the layout system didn't give us a new viewport during the layout, we
            // need to do another layout pass as the one that took place was a no-op.
            if (_isWaitingForViewportUpdate)
            {
                _isWaitingForViewportUpdate = false;
                InvalidateMeasure();
                (root as Layoutable)?.UpdateLayout();
            }

            // During the previous BringIntoView, the scroll width extent might have been out of date if
            // elements have different widths. Because of that, the ScrollViewer might not scroll to the correct offset.
            // After the previous BringIntoView, Y offset should be correct and an extra layout pass has been executed,
            // hence the width extent should be correct now, and we can try to scroll again.
            scrollToElement.BringIntoView();

            if (_scrollToElement is not null)
            {
                RecycleElement(_scrollToElement, _scrollToIndex);
            }

            _scrollToElement = null;
            _scrollToIndex = -1;
            return scrollToElement;
        }

        return null;
    }

    private double GetWrappingWidth()
    {
        var width = GetWidth(_viewport.Size);
        if (width <= 0) width = _lastLayoutWidth;
        if (width <= 0) width = GetWidth(Bounds.Size);
        if (width <= 0) width = GetWidth(DesiredSize);
        if (width <= 0) width = _FallbackItemSize.Width * 10;
        return width;
    }

    /// <summary>
    /// Calculates the desired size of the viewport.
    /// </summary>
    /// <param name="orientation">the <see cref="Orientation"/> to use</param>
    /// <param name="itemCount">The number of items</param>
    /// <param name="wrappingWidth">the width used for wrapping</param>
    /// <returns>the desired size</returns>
    private Size CalculateDesiredSize(Orientation orientation, int itemCount, double wrappingWidth)
    {
        if (itemCount == 0) return _EmptySize;

        var averageItemSize = GetAverageItemSize();

        var itemWidth = GetWidth(averageItemSize);
        var itemHeight = GetHeight(averageItemSize);

        if (itemWidth == 0 || itemHeight == 0) return _EmptySize;

        var itemsPerRow = Math.Max(Math.Floor((wrappingWidth + EPSILON) / itemWidth), 1);

        double sizeU = 0d;
        if (AllowDifferentSizedItems)
        {
            // If we have a partially populated row cache, we can use it to estimate the rest
            if (_rowCache.Count > 0)
            {
                var lastRow = _rowCache[_rowCache.Count - 1];
                var startIndex = lastRow.StartIndex + lastRow.Count;
                var remainingItems = itemCount - startIndex;
                    
                if (remainingItems <= 0)
                {
                    sizeU = lastRow.Y + lastRow.Height;
                }
                else
                {
                    if (ItemSizeProvider is not null)
                    {
                        double x = 0;
                        double rowHeight = 0;
                        double y = lastRow.Y + lastRow.Height;

                        for (int i = startIndex; i < itemCount; i++)
                        {
                            var item = Items[i];
                            Size itemSize = GetAssumedItemSize(i, item);

                            if (x != 0 && x + GetWidth(itemSize) > wrappingWidth)
                            {
                                x = 0;
                                y += rowHeight;
                                rowHeight = 0;
                            }

                            x += GetWidth(itemSize);
                            rowHeight = Math.Max(rowHeight, GetHeight(itemSize));
                        }
                        sizeU = y + rowHeight;
                    }
                    else
                    {
                        // Estimate remaining rows
                        var remainingRows = Math.Ceiling(remainingItems / itemsPerRow);
                        sizeU = lastRow.Y + lastRow.Height + (remainingRows * itemHeight);
                    }
                }
            }
            else
            {
                // Full linear scan fallback only if no cache available
                double x = 0;
                double rowHeight = 0;
                var items = Items;
                var count = items.Count;

                for (int i = 0; i < count; i++)
                {
                    Size itemSize = GetAssumedItemSize(i, items[i]);

                    if (x != 0 && x + GetWidth(itemSize) > wrappingWidth)
                    {
                        x = 0;
                        sizeU += rowHeight;
                        rowHeight = 0;
                    }

                    x += GetWidth(itemSize);
                    rowHeight = Math.Max(rowHeight, GetHeight(itemSize));
                }

                sizeU += rowHeight;
            }
        }
        else
        {
            sizeU = Math.Ceiling(itemCount / itemsPerRow) * itemHeight;
        }

        return orientation == Orientation.Horizontal ?
            new Size(wrappingWidth, sizeU) :
            new Size(sizeU, wrappingWidth);
    }

    /// <summary>
    /// Estimates the desired size
    /// </summary>
    /// <param name="orientation">the <see cref="Orientation"/> to use</param>
    /// <param name="itemCount">The number of items</param>
    /// <param name="wrappingWidth">the width used for wrapping</param>
    /// <returns>the estimated desired size</returns>
    private Size EstimateDesiredSize(Orientation orientation, int itemCount, double wrappingWidth)
    {
        if (_scrollToIndex >= 0 && _scrollToElement is not null)
        {
            // We have an element to scroll to, so we can estimate the desired size based on the
            // element's position and the remaining elements.
            var remainingItems = itemCount - _scrollToIndex - 1;

            if (remainingItems <= 0)
            {
                var u = GetY(_scrollToElement.Bounds.BottomRight);
                return orientation == Orientation.Horizontal ?
                    new(wrappingWidth, u) :
                    new(u, wrappingWidth);
            }

            double sizeU;

            if (AllowDifferentSizedItems && ItemSizeProvider is not null)
            {
                double x = 0;
                double rowHeight = 0;
                double y = 0;

                // Find start of row for _scrollToIndex
                var start = FindItemOffset(_scrollToIndex, wrappingWidth);
                x = GetX(start);
                y = GetY(start);
                
                for (int i = _scrollToIndex; i < itemCount; i++)
                {
                    Size itemSize = (i == _scrollToIndex) ? 
                        new Size(GetWidth(_scrollToElement.Bounds.Size), GetHeight(_scrollToElement.Bounds.Size)) :
                        GetAssumedItemSize(i, Items[i]);

                    if (x != 0 && x + GetWidth(itemSize) > wrappingWidth)
                    {
                        x = 0;
                        y += rowHeight;
                        rowHeight = 0;
                    }

                    x += GetWidth(itemSize);
                    rowHeight = Math.Max(rowHeight, GetHeight(itemSize));
                }
                sizeU = y + rowHeight;
            }
            else
            {
                var avgSize = GetAverageItemSize();
                var avgWidth = GetWidth(avgSize);
                var itemsPerRow = Math.Max(Math.Floor((wrappingWidth + EPSILON) / avgWidth), 1);
                var remainingRows = (int)Math.Ceiling(remainingItems / itemsPerRow);
                var u = GetY(_scrollToElement.Bounds.BottomRight);
                sizeU = u + (remainingRows * GetHeight(avgSize));
            }

            return orientation == Orientation.Horizontal ?
                new(wrappingWidth, sizeU) :
                new(sizeU, wrappingWidth);
        }

        return DesiredSize;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {

        if (change.Property == OrientationProperty)
        {
            ClearRowCache();
            InvalidateMeasure();
            InvalidateArrange();
            ScrollIntoView(0);
        }

        if (change.Property == AllowDifferentSizedItemsProperty || change.Property == ItemSizeProperty ||
            change.Property == IsGridLayoutEnabledProperty || change.Property == StretchItemsProperty ||
            change.Property == CacheRowsProperty)
        {
            foreach (var child in Children)
            {
                child.InvalidateMeasure();
            }

            ClearRowCache();
            InvalidateMeasure();
            InvalidateArrange();
        }


        base.OnPropertyChanged(change);
    }

    /// <summary>
    /// Realizes visible items and virtualizes non-visible items
    /// </summary>
    private void RealizeAndVirtualizeItems()
    {
        FindStartIndexAndOffset();
        VirtualizeItemsBeforeStartIndex();
        RealizeItemsAndFindEndIndex();
        VirtualizeItemsAfterEndIndex();
    }

    /// <summary>
    /// Calculates the predicted average item size
    /// </summary>
    /// <returns>the estimated average Size</returns>
    private Size GetAverageItemSize()
    {
        if (!ItemSize.NearlyEquals(_EmptySize))
        {
            return ItemSize;
        }
        else if (!AllowDifferentSizedItems)
        {
            return _sizeOfFirstItem ?? _FallbackItemSize;
        }
        else
        {
            return _averageItemSizeCache ??= CalculateAverageItemSize();
        }
    }

    /// <summary>
    /// Calculates the start offset for a given item index
    /// </summary>
    /// <param name="itemIndex">the index of the requested item</param>
    /// <param name="wrappingWidth">the width used for wrapping</param>
    /// <returns>the starting point</returns>
    private Point FindItemOffset(int itemIndex, double? wrappingWidth = null)
    {
        double x = 0, y = 0, rowHeight = 0;

        if (!AllowDifferentSizedItems && Items.Count > 0)
        {
            var itemWidth = GetWidth(GetAssumedItemSize(Items[0]));
            var itemHeight = GetHeight(GetAssumedItemSize(Items[0]));

            if (itemWidth == 0 || itemHeight == 0) return new Point();

            var viewportWidth = wrappingWidth ?? GetWidth(_viewport.Size);
            if (viewportWidth <= 0) viewportWidth = _lastLayoutWidth;
            if (viewportWidth <= 0) viewportWidth = GetWidth(Bounds.Size);
            if (viewportWidth <= 0) viewportWidth = GetWidth(DesiredSize);
            if (viewportWidth <= 0) viewportWidth = _FallbackItemSize.Width * 10; // Extreme fallback

            var itemsPerRow = (int)Math.Max(Math.Floor((viewportWidth + EPSILON) / itemWidth), 1);

            var itemRowIndex = (int)Math.Floor(itemIndex * 1.0 / itemsPerRow);
            y = itemRowIndex * itemHeight;

            GetRowLayout(viewportWidth, itemsPerRow, itemsPerRow * itemWidth, out var innerSpacing, out var outerSpacing, out var extraWidth);
            var indexInRow = itemIndex - itemRowIndex * itemsPerRow;
            x = outerSpacing + indexInRow * (itemWidth + extraWidth + innerSpacing);

            return CreatePoint(x, y);
        }

        var effectiveWrappingWidth = wrappingWidth ?? GetWidth(_viewport.Size);
        if (effectiveWrappingWidth <= 0) effectiveWrappingWidth = _lastLayoutWidth;
        if (effectiveWrappingWidth <= 0) effectiveWrappingWidth = GetWidth(Bounds.Size);
        if (effectiveWrappingWidth <= 0) effectiveWrappingWidth = GetWidth(DesiredSize);
        if (effectiveWrappingWidth <= 0) effectiveWrappingWidth = _FallbackItemSize.Width * 10; // Extreme fallback

        int startIndex = 0;

        // Try to use row cache to quickly jump to the correct row and then accumulate within the row
        if (_rowCache.Count > 0)
        {
            int lo = 0, hi = _rowCache.Count - 1, best = -1;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                var r = _rowCache[mid];
                if (r.StartIndex <= itemIndex)
                {
                    best = mid;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            if (best >= 0)
            {
                var row = _rowCache[best];
                y = row.Y;
                rowHeight = row.Height;

                // If the item is within this row, we can calculate its X using row info
                if (itemIndex < row.StartIndex + row.Count)
                {
                    GetRowLayout(effectiveWrappingWidth, row.Count, row.SummedUpChildWidth, out var innerSpacing, out var outerSpacing, out var extraWidth);
                    x = outerSpacing;
                    for (int i = row.StartIndex; i < itemIndex; i++)
                    {
                        var size = GetAssumedItemSize(i, Items[i]);
                        x += GetWidth(size) + extraWidth + innerSpacing;
                    }
                    return CreatePoint(x, y);
                }
                else
                {
                    // Item is beyond this row, continue linear scan from the next item
                    y += rowHeight;
                    rowHeight = 0;
                    startIndex = row.StartIndex + row.Count;
                }
            }
        }

        // Fallback or continuation: linear accumulation
        {
            int currentRowStartIndex = startIndex;
            int currentRowCount = 0;
            double currentRowSummedUpWidth = 0;
            x = 0;

            for (int i = startIndex; i < Items.Count; i++)
            {
                Size itemSize = GetAssumedItemSize(i, Items[i]);
                double itemWidth = GetWidth(itemSize);

                if (currentRowCount > 0 && x + itemWidth > effectiveWrappingWidth + EPSILON)
                {
                    // Current row is finished. Check if target was in it.
                    if (itemIndex < i)
                    {
                        // Target was in the row we just finished.
                        GetRowLayout(effectiveWrappingWidth, currentRowCount, currentRowSummedUpWidth, out var innerSpacing, out var outerSpacing, out var extraWidth);
                        double finalX = outerSpacing;
                        for (int j = currentRowStartIndex; j < itemIndex; j++)
                        {
                            var s = GetAssumedItemSize(j, Items[j]);
                            finalX += GetWidth(s) + extraWidth + innerSpacing;
                        }
                        return CreatePoint(finalX, y);
                    }

                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                    currentRowSummedUpWidth = 0;
                    currentRowStartIndex = i;
                    currentRowCount = 0;
                }

                x += itemWidth;
                currentRowSummedUpWidth += itemWidth;
                rowHeight = Math.Max(rowHeight, GetHeight(itemSize));
                currentRowCount++;

                if (i == itemIndex && i == Items.Count - 1)
                {
                    // It's the last item and it's our target.
                    GetRowLayout(effectiveWrappingWidth, currentRowCount, currentRowSummedUpWidth, out var innerSpacing, out var outerSpacing, out var extraWidth);
                    double finalX = outerSpacing;
                    for (int j = currentRowStartIndex; j < itemIndex; j++)
                    {
                        var s = GetAssumedItemSize(j, Items[j]);
                        finalX += GetWidth(s) + extraWidth + innerSpacing;
                    }
                    return CreatePoint(finalX, y);
                }
            }

            // Check if it's in the last (possibly unfinished) row
            if (itemIndex >= currentRowStartIndex && itemIndex < currentRowStartIndex + currentRowCount)
            {
                GetRowLayout(effectiveWrappingWidth, currentRowCount, currentRowSummedUpWidth, out var innerSpacing, out var outerSpacing, out var extraWidth);
                double finalX = outerSpacing;
                for (int j = currentRowStartIndex; j < itemIndex; j++)
                {
                    var s = GetAssumedItemSize(j, Items[j]);
                    finalX += GetWidth(s) + extraWidth + innerSpacing;
                }
                return CreatePoint(finalX, y);
            }

            return CreatePoint(0, y);
        }
    }

    /// <summary>
    ///  Calculates the anchor index and scroll offset for the anchor
    /// </summary>
    private void FindStartIndexAndOffset()
    {
        double startOffsetY = DetermineStartOffsetY();

        if (startOffsetY <= 0)
        {
            _startItemIndex = Items.Count > 0 ? 0 : -1;
            _startItemOffsetX = 0;
            _startItemOffsetY = 0;
            return;
        }

        _startItemIndex = -1;

        double x = 0, y = 0, rowHeight = 0;
        int indexOfFirstRowItem = 0;

        int itemIndex = 0;
        var wrappingWidth = GetWrappingWidth();

        // Use cached rows if available to quickly resolve the starting row
        if (_rowCache.Count > 0)
        {
            int lo = 0, hi = _rowCache.Count - 1, best = -1;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                var r = _rowCache[mid];
                if (r.Y <= startOffsetY)
                {
                    best = mid;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            if (best >= 0)
            {
                // Found a row that starts at or before startOffsetY
                var foundRow = _rowCache[best];

                if (startOffsetY < foundRow.Y + foundRow.Height)
                {
                    // This row (or one before it) contains startOffsetY
                    var targetIndex = Math.Max(0, best - Math.Max(0, CacheRows));
                    var r = _rowCache[targetIndex];
                    _startItemIndex = r.StartIndex;
                    _startItemOffsetX = 0;
                    _startItemOffsetY = r.Y;
                    return;
                }
                else
                {
                    // startOffsetY is beyond the foundRow, use it as a starting point for linear scan
                    itemIndex = foundRow.StartIndex + foundRow.Count;
                    x = 0;
                    y = foundRow.Y + foundRow.Height;
                    rowHeight = 0;
                    indexOfFirstRowItem = itemIndex;
                }
            }
        }

        if (!AllowDifferentSizedItems && Items.Count > 0)
        {
            var itemWidth = GetWidth(GetAssumedItemSize(Items[0]));
            var itemHeight = GetHeight(GetAssumedItemSize(Items[0]));

            if (itemWidth == 0 || itemHeight == 0) return;

            var itemsPerRow = Math.Max(Math.Floor((wrappingWidth + EPSILON) / itemWidth), 1);

            var startRowIndex = (int)Math.Floor(startOffsetY / itemHeight);
            _startItemIndex = (int)(startRowIndex * itemsPerRow);
            _startItemOffsetX = 0;
            _startItemOffsetY = startRowIndex * itemHeight;

            // Apply CacheRows
            var rowsToMoveUp = Math.Max(0, CacheRows);
            var actualRowsToMoveUp = Math.Min(startRowIndex, rowsToMoveUp);
            _startItemIndex -= (int)(actualRowsToMoveUp * itemsPerRow);
            _startItemOffsetY -= actualRowsToMoveUp * itemHeight;

            return;
        }

        if (AllowDifferentSizedItems && Items.Count > 0)
        {
            var previousRows = new List<(int index, double y)>();

            // Linear scan fallback
            for (; itemIndex < Items.Count; itemIndex++)
            {
                var item = Items[itemIndex];
                Size itemSize = GetAssumedItemSize(itemIndex, item);

                if (x + GetWidth(itemSize) > wrappingWidth && x != 0)
                {
                    previousRows.Add((indexOfFirstRowItem, y));
                    if (previousRows.Count > CacheRows + 1)
                        previousRows.RemoveAt(0);

                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                    indexOfFirstRowItem = itemIndex;
                }

                x += GetWidth(itemSize);
                rowHeight = Math.Max(rowHeight, GetHeight(itemSize));

                if (y + rowHeight > startOffsetY)
                {
                    // Found the row containing startOffsetY. 
                    // Move back by CacheRows if possible.
                    if (previousRows.Count > 0)
                    {
                        var targetRow = previousRows[Math.Max(0, previousRows.Count - Math.Max(0, CacheRows))];
                        _startItemIndex = targetRow.index;
                        _startItemOffsetX = 0;
                        _startItemOffsetY = targetRow.y;
                    }
                    else
                    {
                        _startItemIndex = indexOfFirstRowItem;
                        _startItemOffsetX = 0;
                        _startItemOffsetY = y;
                    }
                    return;
                }
            }
        }

        // make sure that at least one item is realized to allow correct calculation of the extent
        if (_startItemIndex == -1 && Items.Count > 0)
        {
            _startItemIndex = Items.Count - 1;
            _startItemOffsetX = x;
            _startItemOffsetY = y;
        }
    }

    /// <summary>
    /// Realizes all elements until the visible ViewPort is full
    /// </summary>
    private void RealizeItemsAndFindEndIndex()
    {
        if (_startItemIndex == -1)
        {
            _endItemIndex = -1;
            _knownExtentX = 0;
            return;
        }

        int newEndItemIndex = Items.Count - 1;
        bool endItemIndexFound = false;

        double endOffsetY = DetermineEndOffsetY();

        var wrappingWidth = GetWrappingWidth();
        double x = _startItemOffsetX;
        double y = _startItemOffsetY;
        double rowHeight = 0;
        double currentRowSummedUpWidth = 0;
        int currentRowStartIndex = _startItemIndex;
        int currentRowCount = 0;
        bool endRowReached = false;
        int extraRowsToRealize = Math.Max(0, CacheRows);

        _knownExtentX = 0;

        for (int itemIndex = _startItemIndex; itemIndex <= newEndItemIndex; itemIndex++)
        {
            if (itemIndex == 0)
            {
                _sizeOfFirstItem = null;
            }

            object? item = Items[itemIndex];

            var container = GetOrCreateElement(Items, itemIndex);

            if (container == _scrollToElement)
            {
                _scrollToIndex = -1;
                _scrollToElement = null;
            }

            Size? upfrontKnownItemSize = GetUpfrontKnownItemSize(item);

            // Prefer measuring with a concrete size when truly known (ItemSize, _sizeOfFirstItem, or provider).
            // If unknown, use Size.Infinity so the template can produce its natural DesiredSize.
            Size? measureSize = upfrontKnownItemSize
                                ?? _sizeOfFirstItem
                                ?? (!ItemSize.NearlyEquals(_EmptySize) ? ItemSize : (Size?)null);

            // Optimization: Skip Measure if the container already has the correct desired size.
            // However, we MUST measure if the container was just recycled (e.g. from GetOrCreateElement)
            // because it might have a different item now.
            // Avalonia's VirtualizingPanel usually handles this, but since we are doing custom realization:
            container.Measure(measureSize ?? Size.Infinity);

            var containerSize = DetermineContainerSize(item, container, upfrontKnownItemSize);

            if (_measureElements!.GetElement(itemIndex) == null)
            {
                _averageItemSizeCache = null;
                _measureElements!.Add(itemIndex, container, containerSize);
            }

            if (AllowDifferentSizedItems == false && _sizeOfFirstItem is null)
            {
                _sizeOfFirstItem = containerSize;
            }

            if (x != 0 && (x + GetWidth(containerSize)) > (wrappingWidth + EPSILON))
            {
                // finalize previous row in cache
                AddRowCacheEntry(currentRowStartIndex, y, rowHeight, currentRowCount, currentRowSummedUpWidth);

                // If we've already reached the viewport end row earlier, count down extra rows
                if (endRowReached)
                {
                    if (extraRowsToRealize <= 0)
                    {
                        newEndItemIndex = itemIndex - 1;
                        break;
                    }
                    extraRowsToRealize--;
                }

                x = 0;
                y += rowHeight;
                rowHeight = 0;
                currentRowSummedUpWidth = 0;
                currentRowStartIndex = itemIndex;
                currentRowCount = 0;
            }

            x += GetWidth(containerSize);
            currentRowSummedUpWidth += GetWidth(containerSize);
            _knownExtentX = Math.Max(x, _knownExtentX);
            rowHeight = Math.Max(rowHeight, GetHeight(containerSize));
            currentRowCount++;

            if (endItemIndexFound == false)
            {
                if (y >= endOffsetY
                    || (AllowDifferentSizedItems == false
                        && x + GetWidth(_sizeOfFirstItem!.Value) > wrappingWidth
                        && y + rowHeight >= endOffsetY))
                {
                    endItemIndexFound = true;
                    endRowReached = true;
                    newEndItemIndex = itemIndex;
                }
            }
        }

        // finalize last row
        AddRowCacheEntry(currentRowStartIndex, y, rowHeight, currentRowCount, currentRowSummedUpWidth);

        _endItemIndex = newEndItemIndex;
    }

    /// <summary>
    /// Determines the container size
    /// </summary>
    /// <param name="item">the item to use</param>
    /// <param name="container">the container</param>
    /// <param name="upfrontKnownItemSize">the known item size, if any</param>
    /// <returns></returns>
    private Size DetermineContainerSize(object? item, Control container, Size? upfrontKnownItemSize)
    {
        if (ItemSizeProvider is not null && item is not null)
        {
            return ItemSizeProvider.GetSizeForItem(item);
        }

        return upfrontKnownItemSize ?? _realizedElements?.GetElementSize(container) ?? container.DesiredSize;
    }

    /// <summary>
    /// Removes all items that are realized before start index
    /// </summary>
    private void VirtualizeItemsBeforeStartIndex()
    {
        _realizedElements!.RecycleElementsBefore(_startItemIndex, RecycleElement);
    }

    /// <summary>
    /// Removes all items that are realized after start index
    /// </summary>
    private void VirtualizeItemsAfterEndIndex()
    {
        _realizedElements!.RecycleElementsAfter(_endItemIndex, RecycleElement);
    }

    /// <summary>
    /// Calculates the start y-offset of the effective viewport
    /// </summary>
    /// <returns>the y-component of the effective viewport</returns>
    private double DetermineStartOffsetY()
    {
        return Math.Max(GetY(_viewport.TopLeft), 0);
    }

    /// <summary>
    /// Calculates the end y-offset of the effective viewport
    /// </summary>
    /// <returns>the y-component of the effective viewport</returns>
    private double DetermineEndOffsetY()
    {
        return Math.Max(0, GetY(_viewport.BottomRight));
    }

    /// <summary>
    /// Calculates the upfront known item size
    /// </summary>
    /// <param name="item">the item to use</param>
    /// <returns>the size of the item or null if not known</returns>
    private Size? GetUpfrontKnownItemSize(object? item)
    {
        if (item is null)
        {
            return null;
        }

        if (!ItemSize.NearlyEquals(_EmptySize))
        {
            return ItemSize;
        }

        if (!AllowDifferentSizedItems && _sizeOfFirstItem != null)
        {
            return _sizeOfFirstItem;
        }

        if (ItemSizeProvider != null)
        {
            return ItemSizeProvider.GetSizeForItem(item);
        }

        return null;
    }

    /// <summary>
    /// Calculates the assumed item size
    /// </summary>
    /// <param name="index">the index of the item</param>
    /// <param name="item">the item to use</param>
    /// <returns>the assumed size of the item</returns>
    private Size GetAssumedItemSize(int index, object? item)
    {
        if (item is null) return _EmptySize;

        if (GetUpfrontKnownItemSize(item) is { } upfrontKnownItemSize)
        {
            return upfrontKnownItemSize;
        }

        if (_realizedElements!.GetElementSize(index) is { } cachedItemSize)
        {
            return cachedItemSize;
        }

        return GetAverageItemSize();
    }

    /// <summary>
    /// Calculates the assumed item size
    /// </summary>
    /// <param name="item">the item to use</param>
    /// <returns>the assumed size of the item</returns>
    private Size GetAssumedItemSize(object? item)
    {
        if (item is null) return _EmptySize;

        if (GetUpfrontKnownItemSize(item) is { } upfrontKnownItemSize)
        {
            return upfrontKnownItemSize;
        }

        return GetAverageItemSize();
    }

    /// <summary>
    /// Arranges items in a single row
    /// </summary>
    /// <param name="rowWidth">the available row width</param>
    /// <param name="children">the children to arrange</param>
    /// <param name="childSizes">the sizes of the children</param>
    /// <param name="y">the y offset of the row</param>
    /// <param name="summedUpChildWidth">the pre-calculated sum of all children's width</param>
    private void ArrangeRow(double rowWidth, List<Control> children, List<Size> childSizes, double y, double summedUpChildWidth)
    {
        var childCount = children.Count;
        GetRowLayout(rowWidth, childCount, summedUpChildWidth, out var innerSpacing, out var outerSpacing, out var extraWidth);

        double x = -GetX(_viewport.TopLeft) + outerSpacing;
        
        double rowHeight;
        if (AllowDifferentSizedItems)
        {
            rowHeight = 0;
            for (int i = 0; i < childSizes.Count; i++)
            {
                var height = GetHeight(childSizes[i]);
                if (height > rowHeight)
                    rowHeight = height;
            }
        }
        else
        {
            rowHeight = GetHeight(childSizes[0]);
        }

        if (AllowDifferentSizedItems)
        {
            for (int i = 0; i < childCount; i++)
            {
                var child = children[i];
                Size childSize = childSizes[i];
                child.Arrange(CreateRect(x, y, GetWidth(childSize) + extraWidth, rowHeight));
                x += GetWidth(childSize) + extraWidth + innerSpacing;
            }
        }
        else
        {
            double childWidth = GetWidth(childSizes[0]);
            double arrangedWidth = childWidth + extraWidth;
            for (int i = 0; i < childCount; i++)
            {
                var child = children[i];
                child.Arrange(CreateRect(x, y, arrangedWidth, rowHeight));
                x += arrangedWidth + innerSpacing;
            }
        }
    }

    private void GetRowLayout(double rowWidth, int actualChildCount, double summedUpChildWidth,
        out double innerSpacing, out double outerSpacing, out double extraWidthPerItem)
    {
        extraWidthPerItem = 0;
        double effectiveSummedUpWidth = summedUpChildWidth;

        if (StretchItems && actualChildCount > 0)
        {
            if (AllowDifferentSizedItems)
            {
                extraWidthPerItem = (rowWidth - summedUpChildWidth) / actualChildCount;
                effectiveSummedUpWidth = rowWidth;
            }
            else
            {
                var averageSize = GetAverageItemSize();
                var childWidth = GetWidth(averageSize);
                int itemsPerRow = IsGridLayoutEnabled ?
                    (int)Math.Max(1, Math.Floor((rowWidth + EPSILON) / childWidth)) :
                    actualChildCount;

                double stretchedChildWidth = rowWidth / itemsPerRow;
                // Note: We don't have access to children's MaxWidth here easily, 
                // but ArrangeRow handles it if needed. For estimation we use full stretch.
                extraWidthPerItem = stretchedChildWidth - childWidth;
                effectiveSummedUpWidth = itemsPerRow * stretchedChildWidth;
            }
        }

        CalculateRowSpacing(rowWidth, actualChildCount, effectiveSummedUpWidth, out innerSpacing, out outerSpacing);
    }

    /// <summary>
    /// Calculates the row spacing between the items and before and after the row
    /// </summary>
    /// <param name="rowWidth">the available row width</param>
    /// <param name="actualChildCount">the number of children in the row</param>
    /// <param name="summedUpChildWidth">the sum of all children's width</param>
    /// <param name="innerSpacing">returns the spacing between items</param>
    /// <param name="outerSpacing">returns the spacing before and after each row</param>
    private void CalculateRowSpacing(double rowWidth, int actualChildCount, double summedUpChildWidth,
        out double innerSpacing, out double outerSpacing)
    {
        int spacingChildCount = actualChildCount;

        if (!AllowDifferentSizedItems && IsGridLayoutEnabled)
        {
            var averageItemSize = GetAverageItemSize();
            var itemWidth = GetWidth(averageItemSize);
            if (itemWidth > 0)
            {
                spacingChildCount = (int)Math.Max(1, Math.Floor((rowWidth + EPSILON) / itemWidth));
            }
        }

        double unusedWidth = Math.Max(0, rowWidth - summedUpChildWidth);

        switch (SpacingMode)
        {
            case SpacingMode.Uniform:
                innerSpacing = outerSpacing = unusedWidth / (spacingChildCount + 1);
                break;

            case SpacingMode.BetweenItemsOnly:
                innerSpacing = unusedWidth / Math.Max(spacingChildCount - 1, 1);
                outerSpacing = 0;
                break;

            case SpacingMode.StartAndEndOnly:
                innerSpacing = 0;
                outerSpacing = unusedWidth / 2;
                break;

            case SpacingMode.None:
            default:
                innerSpacing = 0;
                outerSpacing = 0;
                break;
        }
    }

    /// <summary>
    /// Calculates the average item size of all realized items
    /// </summary>
    /// <returns>the average item size or <see cref="_FallbackItemSize"/> if no items are available</returns>
    private Size CalculateAverageItemSize()
    {
        var sizes = _realizedElements!.Sizes;
        var count = sizes.Count;

        if (count > 0)
        {
            double totalWidth = 0;
            double totalHeight = 0;

            for (int i = 0; i < count; i++)
            {
                totalWidth += sizes[i].Width;
                totalHeight += sizes[i].Height;
            }

            return new Size(
                Math.Round(totalWidth / count),
                Math.Round(totalHeight / count));
        }

        return _FallbackItemSize;
    }

    /// <summary>
    /// This method gets called when the effective viewport got changed
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">the event args</param>
    private void OnEffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
    {
        // var vertical = Orientation == Orientation.Vertical;
        var oldViewportStartX = GetX(_viewport.TopLeft);
        var oldViewportStartY = GetY(_viewport.TopLeft); // vertical ? ScrollOffset.Top : _viewport.Left;
        var oldViewportEndX = GetX(_viewport.BottomRight);
        var oldViewportEndY = GetY(_viewport.BottomRight); // vertical ? _viewport.Bottom : _viewport.Right;

        _viewport = e.EffectiveViewport;
        _isWaitingForViewportUpdate = false;

        var newViewportStartX = GetX(_viewport.TopLeft);
        var newViewportStartY = GetY(_viewport.TopLeft); // vertical ? _viewport.Top : _viewport.Left;
        var newViewportEndX = GetX(_viewport.BottomRight);
        var newViewportEndY = GetY(_viewport.BottomRight); // ? _viewport.Bottom : _viewport.Right);

        var newViewportWidth = GetWidth(_viewport.Size);

        if (_lastLayoutWidth.IsCloseTo(newViewportWidth))
        {
            // Optimization: Skip InvalidateMeasure if the new viewport is within what we already have realized/cached.
            // This is safe because:
            // 1. We already have the elements in _realizedElements.
            // 2. MeasureOverride would just result in the same _startItemIndex and _endItemIndex.
            // 3. We STILL call InvalidateArrange() because items might need to be repositioned relative to the viewport.

            bool withinCached = _realizedElements != null &&
                                _startItemIndex >= 0 && _endItemIndex >= 0 &&
                                newViewportStartY >= _startItemOffsetY + (CacheRows > 0 ? GetHeight(GetAverageItemSize()) : 0) &&
                                newViewportEndY <= GetY(FindItemOffset(_endItemIndex, newViewportWidth)) +
                                GetHeight(GetAssumedItemSize(_endItemIndex, Items[_endItemIndex])) - (CacheRows > 0 ? GetHeight(GetAverageItemSize()) : 0);

            if (withinCached)
            {
                if (!oldViewportStartX.IsCloseTo(newViewportStartX) ||
                    !oldViewportEndX.IsCloseTo(newViewportEndX) ||
                    !oldViewportStartY.IsCloseTo(newViewportStartY) ||
                    !oldViewportEndY.IsCloseTo(newViewportEndY))
                {
                    InvalidateArrange();
                }

                return;
            }
        }

        _lastLayoutWidth = newViewportWidth;
        ClearRowCache();

        if (!oldViewportStartX.IsCloseTo(newViewportStartX) ||
            !oldViewportEndX.IsCloseTo(newViewportEndX) ||
            !oldViewportStartY.IsCloseTo(newViewportStartY) ||
            !oldViewportEndY.IsCloseTo(newViewportEndY))
        {
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// This method gets called when the associated ItemsControl is changed
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">the event args</param>
    private void OnItemsControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_focusedElement is not null &&
            e.Property == KeyboardNavigation.TabOnceActiveElementProperty &&
            ReferenceEquals(e.GetOldValue<IInputElement?>(), _focusedElement))
        {
            // TabOnceActiveElement has moved away from _focusedElement so we can recycle it.
            RecycleElement(_focusedElement, _focusedIndex);
            _focusedElement = null;
            _focusedIndex = -1;
        }
    }
    
    private void NavigateLeft(ref int currentIndex)
    {
        switch (Orientation)
        {
            case Orientation.Horizontal:
                --currentIndex;
                break;

            case Orientation.Vertical:
                if (AllowDifferentSizedItems)
                {
                    currentIndex = GetIndexInRelativeRow(currentIndex, -1);
                }
                else
                {
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor((GetWidth(_viewport.Size) + EPSILON) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex -= itemsPerRow;
                }
                break;
        }
    }

    private void NavigateRight(ref int currentIndex)
    {
        switch (Orientation)
        {
            case Orientation.Horizontal:
                ++currentIndex;
                break;

            case Orientation.Vertical:
                if (AllowDifferentSizedItems)
                {
                    currentIndex = GetIndexInRelativeRow(currentIndex, 1);
                }
                else
                {
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor((GetWidth(_viewport.Size) + EPSILON) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex += itemsPerRow;
                }
                break;
        }
    }

    private void NavigateUp(ref int currentIndex)
    {
        switch (Orientation)
        {
            case Orientation.Vertical:
                --currentIndex;
                break;
            case Orientation.Horizontal:
                if (AllowDifferentSizedItems)
                {
                    currentIndex = GetIndexInRelativeRow(currentIndex, -1);
                }
                else
                {
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor((GetWidth(_viewport.Size) + EPSILON) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex -= itemsPerRow;
                }
                break;
        }
    }

    private void NavigateDown(ref int currentIndex)
    {
        switch (Orientation)
        {
            case Orientation.Vertical:
                ++currentIndex;
                break;
            case Orientation.Horizontal:
                if (AllowDifferentSizedItems)
                {
                    currentIndex = GetIndexInRelativeRow(currentIndex, 1);
                }
                else
                {
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor((GetWidth(_viewport.Size) + EPSILON) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex += itemsPerRow;
                }
                break;
        }
    }

    private int GetIndexInRelativeRow(int currentIndex, int rowOffset)
    {
        var wrappingWidth = GetWrappingWidth();
        var currentOffset = FindItemOffset(currentIndex, wrappingWidth);
        var currentSize = GetAssumedItemSize(currentIndex, Items[currentIndex]);

        var currentX = GetX(currentOffset);
        var currentY = GetY(currentOffset);
        var rawCurrentWidth = GetWidth(currentSize);

        // Find source row layout to get actual width
        int sourceRowStartIndex = currentIndex;
        double sourceRowSummedUpWidth = 0;
        int sourceRowCount = 0;

        // Search back to start of source row
        while (sourceRowStartIndex > 0 && GetY(FindItemOffset(sourceRowStartIndex - 1, wrappingWidth)).IsCloseTo(currentY))
        {
            sourceRowStartIndex--;
        }

        // Scan forward to end of source row
        int k = sourceRowStartIndex;
        var itemCount = Items.Count;
        while (k < itemCount && GetY(FindItemOffset(k, wrappingWidth)).IsCloseTo(currentY))
        {
            sourceRowSummedUpWidth += GetWidth(GetAssumedItemSize(k, Items[k]));
            sourceRowCount++;
            k++;
        }

        GetRowLayout(wrappingWidth, sourceRowCount, sourceRowSummedUpWidth, out _, out _, out var sourceExtraWidth);
        var currentWidth = rawCurrentWidth + sourceExtraWidth;
        var currentMidX = currentX + currentWidth / 2;

        // Use or initialize navigation anchor
        if (_navigationAnchor.HasValue)
        {
            currentMidX = _navigationAnchor.Value;
        }
        else
        {
            _navigationAnchor = currentMidX;
        }

        int targetRowStartIndex;
        double targetRowY;

        if (rowOffset < 0)
        {
            // Find row above
            int i = currentIndex;
            double y = currentY;
            while (i > 0)
            {
                var offset = FindItemOffset(i - 1, wrappingWidth);
                if (GetY(offset) < y)
                {
                    // Found the row above
                    targetRowY = GetY(offset);
                    // Now find the start of THIS (target) row
                    int j = i - 1;
                    while (j > 0 && GetY(FindItemOffset(j - 1, wrappingWidth)).IsCloseTo(targetRowY))
                    {
                        j--;
                    }
                    targetRowStartIndex = j;
                    goto FindClosestItem;
                }
                i--;
            }
            return currentIndex; // No row above
        }
        else
        {
            // Find row below
            int i = currentIndex;
            double y = currentY;
            while (i < itemCount - 1)
            {
                var offset = FindItemOffset(i + 1, wrappingWidth);
                if (GetY(offset) > y)
                {
                    // Found the row below
                    targetRowStartIndex = i + 1;
                    targetRowY = GetY(offset);
                    goto FindClosestItem;
                }
                i++;
            }
            return currentIndex; // No row below
        }

    FindClosestItem:
        {
            // Collect target row info
            double targetRowSummedUpWidth = 0;
            int targetRowCount = 0;
            int m = targetRowStartIndex;
            while (m < itemCount)
            {
                var offset = FindItemOffset(m, wrappingWidth);
                if (!GetY(offset).IsCloseTo(targetRowY))
                    break;
                targetRowSummedUpWidth += GetWidth(GetAssumedItemSize(m, Items[m]));
                targetRowCount++;
                m++;
            }

            GetRowLayout(wrappingWidth, targetRowCount, targetRowSummedUpWidth,
                out _, out _, out var targetExtraWidth);

            int bestIndex = targetRowStartIndex;
            double maxOverlap = -1;
            double minDiff = double.MaxValue;

            double sourceStart = currentMidX - currentWidth / 2;
            double sourceEnd = currentMidX + currentWidth / 2;

            for (int i = 0; i < targetRowCount; i++)
            {
                int itemIndex = targetRowStartIndex + i;
                var itemOffset = FindItemOffset(itemIndex, wrappingWidth);
                var itemSize = GetAssumedItemSize(itemIndex, Items[itemIndex]);

                var itemX = GetX(itemOffset);
                var itemWidth = GetWidth(itemSize) + targetExtraWidth;
                var itemMidX = itemX + itemWidth / 2;

                // Overlap with source span
                double overlap = Math.Max(0, Math.Min(sourceEnd, itemX + itemWidth) - Math.Max(sourceStart, itemX));
                double diff = Math.Abs(itemMidX - currentMidX);

                if (overlap > maxOverlap + EPSILON || (Math.Abs(overlap - maxOverlap) < EPSILON && diff < minDiff - EPSILON))
                {
                    maxOverlap = overlap;
                    minDiff = diff;
                    bestIndex = itemIndex;
                }
                else if (itemX > sourceEnd && overlap.IsAlmostZero() && maxOverlap > 0)
                {
                    // We are moving away from the source item's span and we already found an overlapping item
                    break;
                }
            }
            return bestIndex;
        }
    }
    
    /// <summary>
    /// Calculates a virtual X-coordinate based on the <see cref="Orientation"/>
    /// </summary>
    private double GetX(Point point) => Orientation == Orientation.Horizontal ? point.X : point.Y;

    /// <summary>
    /// Calculates a virtual Y-coordinate based on the <see cref="Orientation"/>
    /// </summary>
    private double GetY(Point point) => Orientation == Orientation.Horizontal ? point.Y : point.X;

    /// <summary>
    /// Calculates a virtual width-component based on the <see cref="Orientation"/>
    /// </summary>
    private double GetWidth(Size size) => Orientation == Orientation.Horizontal ? size.Width : size.Height;

    /// <summary>
    /// Calculates a virtual height-component based on the <see cref="Orientation"/>
    /// </summary>
    private double GetHeight(Size size) => Orientation == Orientation.Horizontal ? size.Height : size.Width;

    /// <summary>
    /// Creates a virtual Point based on the <see cref="Orientation"/>
    /// </summary>
    private Point CreatePoint(double x, double y) =>
        Orientation == Orientation.Horizontal ? new Point(x, y) : new Point(y, x);

    /// <summary>
    /// Creates a virtual Rect based on the <see cref="Orientation"/>
    /// </summary>
    private Rect CreateRect(double x, double y, double width, double height) =>
        Orientation == Orientation.Horizontal ? new Rect(x, y, width, height) : new Rect(y, x, height, width);


    /// <summary>
    /// Gets an existing container or creates a new one of none was present
    /// </summary>
    /// <param name="items">the available items</param>
    /// <param name="index">the index to create</param>
    /// <returns>the requested container</returns>
    private Control GetOrCreateElement(IReadOnlyList<object?> items, int index)
    {
        Debug.Assert(ItemContainerGenerator is not null);

        if (GetRealizedElement(index, ref _focusedIndex, ref _focusedElement) is { } focusedElement)
        {
            return focusedElement;
        }

        if (GetRealizedElement(index, ref _scrollToIndex, ref _scrollToElement) is { } scrollToElement)
        {
            return scrollToElement;
        }

        if (GetRealizedElement(index) is { } realized)
            return realized;

        var item = items[index];
        var generator = ItemContainerGenerator!;

        if (generator.NeedsContainer(item, index, out var recycleKey))
        {
            return GetRecycledElement(item, index, recycleKey) ??
                   CreateElement(item, index, recycleKey);
        }
        else
        {
            return GetItemAsOwnContainer(item, index);
        }
    }

    /// <summary>
    /// Gets the realized element or null if not available
    /// </summary>
    /// <param name="index">The container index to lookup</param>
    /// <returns>the realized container</returns>
    private Control? GetRealizedElement(int index)
    {
        return _realizedElements?.GetElement(index);
    }

    /// <summary>
    /// Gets the realized element or null if not available
    /// </summary>
    /// <param name="index">The container index to lookup</param>
    /// <param name="specialIndex">the reference to a special index, e.g. <see cref="_focusedIndex"/></param>
    /// <param name="specialElement">the reference to a special element, e.g. <see cref="_focusedElement"/></param>
    /// <returns>the realized container</returns>
    private static Control? GetRealizedElement(
        int index,
        ref int specialIndex,
        ref Control? specialElement)
    {
        if (specialIndex == index)
        {
            Debug.Assert(specialElement is not null);

            var result = specialElement;
            specialIndex = -1;
            specialElement = null;
            return result;
        }

        return null;
    }

    /// <summary>
    /// Prepares a container if the item is its own container
    /// </summary>
    /// <param name="item">the item to use</param>
    /// <param name="index">the item index</param>
    /// <returns>the prepared container</returns>
    private Control GetItemAsOwnContainer(object? item, int index)
    {
        Debug.Assert(ItemContainerGenerator is not null);

        var controlItem = (Control)item!;
        var generator = ItemContainerGenerator!;

        if (!controlItem.IsSet(_RecycleKeyProperty))
        {
            generator.PrepareItemContainer(controlItem, controlItem, index);
            AddInternalChild(controlItem);
            controlItem.SetValue(_RecycleKeyProperty, s_itemIsItsOwnContainer);
            generator.ItemContainerPrepared(controlItem, item, index);
        }

        controlItem.SetCurrentValue(Visual.IsVisibleProperty, true);
        return controlItem;
    }

    /// <summary>
    /// Gets a recycled container or null if no container to recycle was available
    /// </summary>
    /// <param name="item">the item which uses the container</param>
    /// <param name="index">the item index</param>
    /// <param name="recycleKey">the recycle key</param>
    /// <returns>the recycled container</returns>
    private Control? GetRecycledElement(object? item, int index, object? recycleKey)
    {
        Debug.Assert(ItemContainerGenerator is not null);

        if (recycleKey is null)
            return null;

        var generator = ItemContainerGenerator!;

        if (_recyclePool?.TryGetValue(recycleKey, out var recyclePool) == true && recyclePool.Count > 0)
        {
            var recycled = recyclePool.Pop();
            recycled.SetCurrentValue(Visual.IsVisibleProperty, true);
            generator.PrepareItemContainer(recycled, item, index);
            generator.ItemContainerPrepared(recycled, item, index);
            return recycled;
        }

        return null;
    }

    /// <summary>
    /// Creates a container for a given item
    /// </summary>
    /// <param name="item">the item which needs a container</param>
    /// <param name="index">the item index</param>
    /// <param name="recycleKey">the recycle key to use</param>
    /// <returns>the created element</returns>
    private Control CreateElement(object? item, int index, object? recycleKey)
    {
        Debug.Assert(ItemContainerGenerator is not null);

        var generator = ItemContainerGenerator!;
        var container = generator.CreateContainer(item, index, recycleKey);

        container.SetValue(_RecycleKeyProperty, recycleKey);
        generator.PrepareItemContainer(container, item, index);
        AddInternalChild(container);
        generator.ItemContainerPrepared(container, item, index);

        return container;
    }

    /// <summary>
    /// Recycles a container
    /// </summary>
    /// <param name="element">the container to recycle</param>
    /// <param name="index">the item index</param>
    private void RecycleElement(Control element, int index)
    {
        Debug.Assert(ItemsControl is not null);
        Debug.Assert(ItemContainerGenerator is not null);

        _scrollAnchorProvider?.UnregisterAnchorCandidate(element);

        var recycleKey = element.GetValue(_RecycleKeyProperty);

        if (recycleKey is null)
        {
            RemoveInternalChild(element);
        }
        else if (recycleKey == s_itemIsItsOwnContainer)
        {
            element.SetCurrentValue(Visual.IsVisibleProperty, false);
        }
        else if (ReferenceEquals(KeyboardNavigation.GetTabOnceActiveElement(ItemsControl), element))
        {
            _focusedElement = element;
            _focusedIndex = index;
        }
        else
        {
            ItemContainerGenerator!.ClearItemContainer(element);
            PushToRecyclePool(recycleKey, element);
            element.SetCurrentValue(Visual.IsVisibleProperty, false);
        }
    }

    /// <summary>
    /// Recycles a container if the item was removed
    /// </summary>
    /// <param name="element">the container to recycle</param>
    private void RecycleElementOnItemRemoved(Control element)
    {
        Debug.Assert(ItemContainerGenerator is not null);

        var recycleKey = element.GetValue(_RecycleKeyProperty);

        if (recycleKey is null || recycleKey == s_itemIsItsOwnContainer)
        {
            RemoveInternalChild(element);
        }
        else
        {
            // RemoveInternalChild(element);
            ItemContainerGenerator!.ClearItemContainer(element);
            PushToRecyclePool(recycleKey, element);
            element.SetCurrentValue(Visual.IsVisibleProperty, false);
        }
    }

    /// <summary>
    /// Pushes a container to the recycle pool
    /// </summary>
    /// <param name="recycleKey">the containers recycle-key</param>
    /// <param name="element">the container to recycle</param>
    private void PushToRecyclePool(object recycleKey, Control element)
    {
        _recyclePool ??= new();

        if (!_recyclePool.TryGetValue(recycleKey, out var pool))
        {
            pool = new();
            _recyclePool.Add(recycleKey, pool);
        }

        pool.Push(element);
    }

    /// <summary>
    /// Updates the index of an element
    /// </summary>
    /// <param name="element">the affected element</param>
    /// <param name="oldIndex">the old index</param>
    /// <param name="newIndex">the new index</param>
    private void UpdateElementIndex(Control element, int oldIndex, int newIndex)
    {
        Debug.Assert(ItemContainerGenerator is not null);

        ItemContainerGenerator.ItemContainerIndexChanged(element, oldIndex, newIndex);
    }

    /// <summary>
    /// Defines the <see cref="AreHorizontalSnapPointsRegular"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> AreHorizontalSnapPointsRegularProperty =
        StackPanel.AreHorizontalSnapPointsRegularProperty.AddOwner<VirtualizingWrapPanel>();

    /// <summary>
    /// Defines the <see cref="AreVerticalSnapPointsRegular"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> AreVerticalSnapPointsRegularProperty =
        StackPanel.AreVerticalSnapPointsRegularProperty.AddOwner<VirtualizingWrapPanel>();

    /// <summary>
    /// Defines the <see cref="HorizontalSnapPointsChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> HorizontalSnapPointsChangedEvent =
        RoutedEvent.Register<VirtualizingWrapPanel, RoutedEventArgs>(
            nameof(HorizontalSnapPointsChanged),
            RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="VerticalSnapPointsChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> VerticalSnapPointsChangedEvent =
        RoutedEvent.Register<VirtualizingWrapPanel, RoutedEventArgs>(
            nameof(VerticalSnapPointsChanged),
            RoutingStrategies.Bubble);

    /// <inheritdoc/>
    public Size GetSizeForItem(object item)
    {
        return GetUpfrontKnownItemSize(item) ?? GetAssumedItemSize(item);
    }

    /// <inheritdoc/>
    public IReadOnlyList<double> GetIrregularSnapPoints(Orientation orientation,
        SnapPointsAlignment snapPointsAlignment)
    {
        if (_realizedElements is null)
        {
            return Array.Empty<double>();
        }

        return new VirtualizingWrapPanelSnapPointList(
            _realizedElements,
            Items.Count,
            orientation,
            Orientation,
            snapPointsAlignment,
            GetWidth(GetAverageItemSize()), // This is slightly wrong as it depends on orientation, but VirtualizingWrapPanelSnapPointList seems to want one size
            this as IItemSizeProvider);
    }

    /// <inheritdoc/>
    public double GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment,
        out double offset)
    {
        offset = 0f;
        var firstChild = GetRealizedContainers()?.FirstOrDefault();

        if (firstChild == null)
        {
            return 0;
        }

        double snapPoint = 0;

        switch (Orientation)
        {
            case Orientation.Horizontal:
                if (!AreHorizontalSnapPointsRegular)
                    throw new InvalidOperationException();

                snapPoint = firstChild.Bounds.Width;
                switch (snapPointsAlignment)
                {
                    case SnapPointsAlignment.Near:
                        offset = firstChild.Bounds.Left;
                        break;
                    case SnapPointsAlignment.Center:
                        offset = firstChild.Bounds.Center.X;
                        break;
                    case SnapPointsAlignment.Far:
                        offset = firstChild.Bounds.Right;
                        break;
                }

                break;
            case Orientation.Vertical:
                if (!AreVerticalSnapPointsRegular)
                    throw new InvalidOperationException();
                snapPoint = firstChild.Bounds.Height;
                switch (snapPointsAlignment)
                {
                    case SnapPointsAlignment.Near:
                        offset = firstChild.Bounds.Top;
                        break;
                    case SnapPointsAlignment.Center:
                        offset = firstChild.Bounds.Center.Y;
                        break;
                    case SnapPointsAlignment.Far:
                        offset = firstChild.Bounds.Bottom;
                        break;
                }

                break;
        }

        return snapPoint;
    }

    /// <summary>
    /// Occurs when the measurements for horizontal snap points change.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? HorizontalSnapPointsChanged
    {
        add => AddHandler(HorizontalSnapPointsChangedEvent, value);
        remove => RemoveHandler(HorizontalSnapPointsChangedEvent, value);
    }

    /// <summary>
    /// Occurs when the measurements for vertical snap points change.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? VerticalSnapPointsChanged
    {
        add => AddHandler(VerticalSnapPointsChangedEvent, value);
        remove => RemoveHandler(VerticalSnapPointsChangedEvent, value);
    }

    /// <summary>
    /// Gets or sets whether the horizontal snap points for the <see cref="StackPanel"/> are equidistant from each other.
    /// </summary>
    public bool AreHorizontalSnapPointsRegular
    {
        get => GetValue(AreHorizontalSnapPointsRegularProperty);
        set => SetValue(AreHorizontalSnapPointsRegularProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the vertical snap points for the <see cref="StackPanel"/> are equidistant from each other.
    /// </summary>
    public bool AreVerticalSnapPointsRegular
    {
        get => GetValue(AreVerticalSnapPointsRegularProperty);
        set => SetValue(AreVerticalSnapPointsRegularProperty, value);
    }
}
