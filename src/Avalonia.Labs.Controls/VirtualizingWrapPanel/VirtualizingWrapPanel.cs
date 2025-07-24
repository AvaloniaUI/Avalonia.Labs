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

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// A implementation of a wrap panel that supports virtualization and can be used in horizontal and vertical orientation.
    /// </summary>
    public class VirtualizingWrapPanel : VirtualizingPanel, IScrollSnapPointsInfo
    {
        /// <summary>
        /// Gets an empty size
        /// </summary>
        private static readonly Size EmptySize = new Size(0, 0);

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
        /// Defines the <see cref="Orientation"/> property
        /// </summary>
        public static readonly StyledProperty<Orientation> OrientationProperty =
            WrapPanel.OrientationProperty.AddOwner<VirtualizingWrapPanel>(
                new StyledPropertyMetadata<Orientation>(Orientation.Horizontal));

        /// <summary>
        /// Defines the <see cref="ItemSize"/> property
        /// </summary>
        public static readonly StyledProperty<Size> ItemSizeProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, Size>(nameof(ItemSize), EmptySize);

        /// <summary>
        /// Defines the <see cref="AllowDifferentSizedItems"/> property
        /// </summary>
        public static readonly StyledProperty<bool> AllowDifferentSizedItemsProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(nameof(AllowDifferentSizedItems));

        /// <summary>
        /// Defines the <see cref="ItemSizeProvider"/> property
        /// </summary>
        public static readonly StyledProperty<IItemSizeProvider?> ItemSizeProviderProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, IItemSizeProvider?>(nameof(ItemSizeProvider));

        /// <summary>
        /// Defines the <see cref="SpacingMode"/> property
        /// </summary>
        public static readonly StyledProperty<SpacingMode> SpacingModeProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, SpacingMode>(nameof(SpacingMode), SpacingMode.Uniform);

        /// <summary>
        /// Defines the <see cref="StretchItems"/> property
        /// </summary>
        public static readonly StyledProperty<bool> StretchItemsProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(nameof(StretchItems));

        /// <summary>
        /// Defines the <see cref="IsGridLayoutEnabled"/> property
        /// </summary>
        public static readonly StyledProperty<bool> IsGridLayoutEnabledProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(nameof(IsGridLayoutEnabled), true);

        /// <summary>
        /// Defines the <c>RecycleKey</c> attached property
        /// </summary>
        private static readonly AttachedProperty<object?> RecycleKeyProperty =
            AvaloniaProperty.RegisterAttached<VirtualizingStackPanel, Control, object?>("RecycleKey");

        // ReSharper disable once InconsistentNaming
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
        private int _focusedIndex = -1;

        /// <summary>
        /// Creates a new VirtualizingWrapPanel
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
        /// Gets or sets a value that specifies the size of the items. The default value is <see cref="EmptySize"/>. 
        /// If the value is <see cref="EmptySize"/> the item size is determined by measuring the first realized item.
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
        /// Gets the index of the first realized element, or -1 if no elements are realized.
        /// </summary>
        public int FirstRealizedIndex => _realizedElements?.FirstIndex ?? -1;

        /// <summary>
        /// Gets the index of the last realized element, or -1 if no elements are realized.
        /// </summary>
        public int LastRealizedIndex => _realizedElements?.LastIndex ?? -1;

        /// <summary>
        /// The fallback size in case size calculation went wrong
        /// </summary>
        private static readonly Size FallbackItemSize = new Size(48, 48);

        private Size? _sizeOfFirstItem;

        private Size? _averageItemSizeCache;

        private int _startItemIndex = -1;
        private int _endItemIndex = -1;

        private double _startItemOffsetX;
        private double _startItemOffsetY;

        private double _knownExtendX;

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            var items = Items;

            if (items.Count == 0)
                return default;

            var orientation = Orientation;

            // If we're bringing an item into view, ignore any layout passes until we receive a new
            // effective viewport.
            if (_isWaitingForViewportUpdate)
                return EstimateDesiredSize(orientation, items.Count);

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

                return CalculateDesiredSize(orientation, items.Count);
            }
            finally
            {
                _isInLayout = false;
            }
        }

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
                var rowChilds = new List<Control>();
                var childSizes = new List<Size>();

                for (int i = _startItemIndex; i <= _endItemIndex; i++)
                {
                    var item = Items[i];
                    var child = _realizedElements.GetElement(i);

                    Size? upfrontKnownItemSize = GetUpfrontKnownItemSize(item);

                    Size childSize = upfrontKnownItemSize ??
                                     _realizedElements.GetElementSize(child) ?? FallbackItemSize;

                    if (rowChilds.Count > 0 && x + GetWidth(childSize) > GetWidth(finalSize))
                    {
                        ArrangeRow(GetWidth(finalSize), rowChilds, childSizes, y);
                        x = 0;
                        y += childSizes.Max(x1 => GetHeight(x1));
                        rowHeight = 0;
                        rowChilds.Clear();
                        childSizes.Clear();
                    }

                    x += GetWidth(childSize);
                    rowHeight = Math.Max(rowHeight, GetHeight(childSize));
                    if (child != null)
                    {
                        rowChilds.Add(child);
                        childSizes.Add(childSize);

                        _scrollAnchorProvider?.RegisterAnchorCandidate(child);
                    }
                }

                if (rowChilds.Any())
                {
                    ArrangeRow(GetWidth(finalSize), rowChilds, childSizes, y);
                }

                // Ensure that the focused element is in the correct position.                
                if (_focusedElement is not null && _focusedIndex >= 0)
                {
                    var startPoint = FindItemOffset(_focusedIndex);

                    _startItemOffsetX = GetX(startPoint);
                    _startItemOffsetY = GetY(startPoint);

                    var rect = Orientation == Orientation.Horizontal 
                        ? new Rect(_startItemOffsetX, _startItemOffsetY, _focusedElement.DesiredSize.Width, _focusedElement.DesiredSize.Height) 
                        : new Rect(_startItemOffsetY, _startItemOffsetX, _focusedElement.DesiredSize.Width, _focusedElement.DesiredSize.Height);
                    _focusedElement.Arrange(rect);
                }

                // Ensure that the scrollTo element is in the correct position.                
                if (_scrollToElement is not null && _scrollToIndex >= 0)
                {
                    var startPoint = FindItemOffset(_scrollToIndex);

                    _startItemOffsetX = GetX(startPoint);
                    _startItemOffsetY = GetY(startPoint);

                    var rect = Orientation == Orientation.Horizontal ?
                        new Rect(_startItemOffsetX, _startItemOffsetY, _scrollToElement.DesiredSize.Width,
                            finalSize.Height) :
                        new Rect(_startItemOffsetY, _startItemOffsetX, finalSize.Width,
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
                return from;

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

            return ScrollIntoView(toIndex);
        }

        /// <inheritdoc />
        protected override IEnumerable<Control>? GetRealizedContainers()
        {
            return _realizedElements?.Elements.Where(x => x is not null).Select(x => x!);
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
            if (Items[index] is Control c && c.GetValue(RecycleKeyProperty) == s_itemIsItsOwnContainer)
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

            if (GetRealizedElement(index) is { } element)
            {
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
                var start = FindItemOffset(index);
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

        /// <summary>
        /// Calculates the desired size of the viewport.
        /// </summary>
        /// <param name="orientation">the <see cref="Orientation"/> to use</param>
        /// <param name="itemCount">The number of items</param>
        /// <returns>the desired size</returns>
        private Size CalculateDesiredSize(Orientation orientation, int itemCount)
        {
            if (itemCount == 0) return EmptySize;

            var avarageItemSize = GetAverageItemSize();

            var viewportWidth = GetWidth(_viewport.Size);

            var itemWidth = GetWidth(avarageItemSize);
            var itemHeight = GetHeight(avarageItemSize);

            if (itemWidth == 0 || itemHeight == 0) return EmptySize;

            var itemsPerRow = Math.Max(Math.Floor(viewportWidth / itemWidth), 1);

            double sizeU = 0d;
            if (AllowDifferentSizedItems)
            {
                double x = 0;
                double rowHeight = 0;

                foreach (var item in Items)
                {
                    Size itemSize = GetAssumedItemSize(item);

                    if (x + GetWidth(itemSize) > GetWidth(_viewport.Size) && x != 0)
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
            else
            {
                sizeU = Math.Ceiling(Items.Count / itemsPerRow) * itemHeight;
            }

            return orientation == Orientation.Horizontal ?
                new Size(viewportWidth, sizeU) :
                new Size(sizeU, viewportWidth);
        }

        /// <summary>
        /// Estimates the desired size
        /// </summary>
        /// <param name="orientation">the <see cref="Orientation"/> to use</param>
        /// <param name="itemCount">The number of items</param>
        /// <returns>the estimated desired size</returns>
        private Size EstimateDesiredSize(Orientation orientation, int itemCount)
        {
            if (_scrollToIndex >= 0 && _scrollToElement is not null)
            {
                // We have an element to scroll to, so we can estimate the desired size based on the
                // element's position and the remaining elements.
                var remainingItems = itemCount - _scrollToIndex - 1;
                var itemsPerRow = Math.Max(Math.Floor(GetWidth(_viewport.Size) / GetWidth(GetAverageItemSize())), 1);
                var remainingRows = (int)Math.Ceiling(remainingItems / itemsPerRow);
                var u = GetY(_scrollToElement.Bounds.BottomRight);
                var sizeU = u + (remainingRows * GetHeight(GetAverageItemSize()));
                return orientation == Orientation.Horizontal ?
                    new(sizeU, DesiredSize.Height) :
                    new(DesiredSize.Width, sizeU);
            }

            return DesiredSize;
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property == OrientationProperty)
            {
                ScrollIntoView(0);
                InvalidateMeasure();
                InvalidateArrange();
            }

            if (change.Property == AllowDifferentSizedItemsProperty || change.Property == ItemSizeProperty ||
                change.Property == IsGridLayoutEnabledProperty || change.Property == StretchItemsProperty)
            {
                foreach (var child in Children)
                {
                    child.InvalidateMeasure();
                }

                InvalidateMeasure();
                InvalidateArrange();
            }
            
            
            base.OnPropertyChanged(change);
        }

        /// <summary>
        /// Realizes visible items and virtualizes non visible items
        /// </summary>
        private void RealizeAndVirtualizeItems()
        {
            FindStartIndexAndOffset();
            VirtualizeItemsBeforeStartIndex();
            RealizeItemsAndFindEndIndex();
            VirtualizeItemsAfterEndIndex();
        }

        /// <summary>
        /// Calculates the predicted avarage item size
        /// </summary>
        /// <returns>the estimated avarage Size</returns>
        private Size GetAverageItemSize()
        {
            if (!ItemSize.NearlyEquals(EmptySize))
            {
                return ItemSize;
            }
            else if (!AllowDifferentSizedItems)
            {
                return _sizeOfFirstItem ?? FallbackItemSize;
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
        /// <returns>the starting point</returns>
        private Point FindItemOffset(int itemIndex)
        {
            double x = 0, y = 0, rowHeight = 0;

            if (!AllowDifferentSizedItems && Items.Count > 0)
            {
                var itemWidth = GetWidth(GetAssumedItemSize(Items[0]));
                var itemHeight = GetHeight(GetAssumedItemSize(Items[0]));

                if (itemWidth == 0 || itemHeight == 0) return new Point();

                var itemsPerRow = Math.Max(Math.Floor(GetWidth(_viewport.Size) / itemWidth), 1);

                var itemRowIndex = (int)Math.Floor(itemIndex * 1.0 / itemsPerRow);
                x = (itemIndex - itemRowIndex * itemsPerRow) * itemWidth;
                y = itemRowIndex * itemHeight;
                return CreatePoint(x, y);
            }

            for (int i = 0; i <= itemIndex; i++)
            {
                Size itemSize = GetAssumedItemSize(Items[i]);

                if (x != 0 && x + GetWidth(itemSize) > GetWidth(_viewport.Size))
                {
                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }

                if (i != itemIndex)
                {
                    x += GetWidth(itemSize);
                    rowHeight = Math.Max(rowHeight, GetHeight(itemSize));
                }
            }

            return CreatePoint(x, y);
        }

        /// <summary>
        ///  Calculates the anchor index and scroll offset for the anchor
        /// </summary>
        private void FindStartIndexAndOffset()
        {
            if (GetY(_viewport.TopLeft) == 0 && GetY(_viewport.BottomRight) == 0)
            {
                _startItemIndex = -1;
                _startItemOffsetX = 0;
                _startItemOffsetY = 0;
                return;
            }

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

            if (!AllowDifferentSizedItems && Items.Count > 0)
            {
                var itemWidth = GetWidth(GetAssumedItemSize(Items[0]));
                var itemHeight = GetHeight(GetAssumedItemSize(Items[0]));

                if (itemWidth == 0 || itemHeight == 0) return;

                var itemsPerRow = Math.Max(Math.Floor(GetWidth(_viewport.Size) / itemWidth), 1);

                var startRowIndex = (int)Math.Floor(startOffsetY / itemHeight);
                _startItemIndex = (int)(startRowIndex * itemsPerRow);
                _startItemOffsetX = 0;
                _startItemOffsetY = startRowIndex * itemHeight;
                return;
            }

            foreach (var item in Items)
            {
                Size itemSize = GetAssumedItemSize(item);

                if (x + GetWidth(itemSize) > GetWidth(_viewport.Size) && x != 0)
                {
                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                    indexOfFirstRowItem = itemIndex;
                }

                x += GetWidth(itemSize);
                rowHeight = Math.Max(rowHeight, GetHeight(itemSize));

                if (y + rowHeight > startOffsetY)
                {
                    _startItemIndex = indexOfFirstRowItem;
                    _startItemOffsetX = 0;
                    _startItemOffsetY = y;
                    break;
                }

                itemIndex++;
            }

            // make sure that at least one item is realized to allow correct calculation of the extend
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
                _knownExtendX = 0;
                return;
            }

            int newEndItemIndex = Items.Count - 1;
            bool endItemIndexFound = false;

            double endOffsetY = DetermineEndOffsetY();

            double x = _startItemOffsetX;
            double y = _startItemOffsetY;
            double rowHeight = 0;

            _knownExtendX = 0;

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

                container.Measure(upfrontKnownItemSize ?? Size.Infinity);

                var containerSize = DetermineContainerSize(item, container, upfrontKnownItemSize);

                _measureElements!.Add(itemIndex, container, containerSize);

                if (AllowDifferentSizedItems == false && _sizeOfFirstItem is null)
                {
                    _sizeOfFirstItem = containerSize;
                }

                if (x != 0 && x + GetWidth(containerSize) > GetWidth(_viewport.Size))
                {
                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }

                x += GetWidth(containerSize);
                _knownExtendX = Math.Max(x, _knownExtendX);
                rowHeight = Math.Max(rowHeight, GetHeight(containerSize));

                if (endItemIndexFound == false)
                {
                    if (y >= endOffsetY
                        || (AllowDifferentSizedItems == false
                            && x + GetWidth(_sizeOfFirstItem!.Value) > GetWidth(_viewport.Size)
                            && y + rowHeight >= endOffsetY))
                    {
                        endItemIndexFound = true;

                        newEndItemIndex = itemIndex;
                    }
                }
            }

            _endItemIndex = newEndItemIndex;
            Debug.WriteLine($"Start: {_startItemIndex} - End: {_endItemIndex}");
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

            if (!ItemSize.NearlyEquals(EmptySize))
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
        /// <param name="item">the item to use</param>
        /// <returns>the assumed size of the item</returns>
        private Size GetAssumedItemSize(object? item)
        {
            if (item is null) return EmptySize;

            if (GetUpfrontKnownItemSize(item) is { } upfrontKnownItemSize)
            {
                return upfrontKnownItemSize;
            }

            var index = Items.IndexOf(item);
            if (_realizedElements!.GetElementSize(index) is { } cachedItemSize)
            {
                return cachedItemSize;
            }

            return GetAverageItemSize();
        }

        /// <summary>
        /// Arranges items in a single row
        /// </summary>
        /// <param name="rowWidth">the available row width</param>
        /// <param name="children">the children to arrange</param>
        /// <param name="childSizes">the sizes of the childres</param>
        /// <param name="y">the y offset of the row</param>
        private void ArrangeRow(double rowWidth, List<Control> children, List<Size> childSizes, double y)
        {
            double summedUpChildWidth;
            double extraWidth = 0;

            if (AllowDifferentSizedItems)
            {
                summedUpChildWidth = childSizes.Sum(childSize => GetWidth(childSize));

                if (StretchItems)
                {
                    double unusedWidth = rowWidth - summedUpChildWidth;
                    extraWidth = unusedWidth / children.Count;
                    summedUpChildWidth = rowWidth;
                }
            }
            else
            {
                double childWidth = GetWidth(childSizes[0]);
                int itemsPerRow = IsGridLayoutEnabled ?
                    (int)Math.Max(Math.Floor(rowWidth / childWidth), 1) :
                    children.Count;

                if (StretchItems)
                {
                    var firstChild = children[0];
                    double maxWidth = Orientation == Orientation.Horizontal ?
                        firstChild.MaxWidth :
                        firstChild.MaxHeight;
                    double stretchedChildWidth = Math.Min(rowWidth / itemsPerRow, maxWidth);
                    stretchedChildWidth =
                        Math.Max(stretchedChildWidth, childWidth); // ItemSize might be greater than MaxWidth/MaxHeight
                    extraWidth = stretchedChildWidth - childWidth;
                    summedUpChildWidth = itemsPerRow * stretchedChildWidth;
                }
                else
                {
                    summedUpChildWidth = itemsPerRow * childWidth;
                }
            }

            double innerSpacing = 0;
            double outerSpacing = 0;

            if (summedUpChildWidth < rowWidth)
            {
                CalculateRowSpacing(rowWidth, children, summedUpChildWidth, out innerSpacing, out outerSpacing);
            }

            double x = -GetX(_viewport.TopLeft) + outerSpacing;

            double rowHeight = childSizes.Max(childSize => GetHeight(childSize));

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                Size childSize = childSizes[i];
                child.Arrange(CreateRect(x, y, GetWidth(childSize) + extraWidth, rowHeight));
                x += GetWidth(childSize) + extraWidth + innerSpacing;
            }
        }

        /// <summary>
        /// Calculates the row spacing between the items and before and after the row
        /// </summary>
        /// <param name="rowWidth">the available row width</param>
        /// <param name="children">the children to consider</param>
        /// <param name="summedUpChildWidth">the sum of all children's width</param>
        /// <param name="innerSpacing">returns the spacing between items</param>
        /// <param name="outerSpacing">returns the spacing before and after each row</param>
        private void CalculateRowSpacing(double rowWidth, List<Control> children, double summedUpChildWidth,
            out double innerSpacing, out double outerSpacing)
        {
            int childCount;

            if (AllowDifferentSizedItems)
            {
                childCount = children.Count;
            }
            else
            {
                childCount = IsGridLayoutEnabled ?
                    (int)Math.Max(1, Math.Floor(rowWidth / GetWidth(_sizeOfFirstItem!.Value))) :
                    children.Count;
            }

            double unusedWidth = Math.Max(0, rowWidth - summedUpChildWidth);

            switch (SpacingMode)
            {
                case SpacingMode.Uniform:
                    innerSpacing = outerSpacing = unusedWidth / (childCount + 1);
                    break;

                case SpacingMode.BetweenItemsOnly:
                    innerSpacing = unusedWidth / Math.Max(childCount - 1, 1);
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
        /// Calculates the avarage item size of all realized items
        /// </summary>
        /// <returns>the avarage item size or <see cref="FallbackItemSize"/> if no items are available</returns>
        private Size CalculateAverageItemSize()
        {
            if (_realizedElements!.Sizes.Count > 0)
            {
                return new Size(
                    Math.Round(_realizedElements!.Sizes.Average(size => size.Width)),
                    Math.Round(_realizedElements!.Sizes.Average(size => size.Height)));
            }

            return FallbackItemSize;
        }

        /// <summary>
        /// This method gets called when the effective viewport got changed
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">the event args</param>
        private void OnEffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
        {
            // var vertical = Orientation == Orientation.Vertical;
            var oldViewportStart = GetY(_viewport.TopLeft); // vertical ? ScrollOffset.Top : _viewport.Left;
            var oldViewportEnd = GetY(_viewport.BottomRight); // vertical ? _viewport.Bottom : _viewport.Right;

            _viewport = e.EffectiveViewport.Intersect(new(Bounds.Size));
            _isWaitingForViewportUpdate = false;

            var newViewportStart = GetY(_viewport.TopLeft); // vertical ? _viewport.Top : _viewport.Left;
            var newViewportEnd = GetY(_viewport.BottomRight); // ? _viewport.Bottom : _viewport.Right);

            if (!oldViewportStart.IsCloseTo(newViewportStart) ||
                !oldViewportEnd.IsCloseTo(newViewportEnd))
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

        #region scroll info

        /// <summary>
        /// This method is called if the user wants to navigate left.
        /// </summary>
        /// <param name="currentIndex">the current index. Update this parameter to set the new index.</param>
        /// <remarks>
        /// Override this method if you want to configure how navigation works. Especially useful if
        /// <see cref="AllowDifferentSizedItems"/> is <see langword="true"></see>
        /// </remarks>
        protected void NavigateLeft(ref int currentIndex)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    --currentIndex;
                    break;

                case Orientation.Vertical:
                    if (AllowDifferentSizedItems) return;
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor(GetWidth(_viewport.Size) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex -= itemsPerRow;
                    break;
            }
        }

        /// <summary>
        /// This method is called if the user wants to navigate right.
        /// </summary>
        /// <param name="currentIndex">the current index. Update this parameter to set the new index.</param>
        /// <remarks>
        /// Override this method if you want to configure how navigation works. Especially useful if
        /// <see cref="AllowDifferentSizedItems"/> is <see langword="true"></see>
        /// </remarks>
        protected void NavigateRight(ref int currentIndex)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    ++currentIndex;
                    break;

                case Orientation.Vertical:
                    if (AllowDifferentSizedItems) return;
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor(GetWidth(_viewport.Size) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex += itemsPerRow;
                    break;
            }
        }

        /// <summary>
        /// This method is called if the user wants to navigate up.
        /// </summary>
        /// <param name="currentIndex">the current index. Update this parameter to set the new index.</param>
        /// <remarks>
        /// Override this method if you want to configure how navigation works. Especially useful if
        /// <see cref="AllowDifferentSizedItems"/> is <see langword="true"></see>
        /// </remarks>
        protected void NavigateUp(ref int currentIndex)
        {
            switch (Orientation)
            {
                case Orientation.Vertical:
                    --currentIndex;
                    break;
                case Orientation.Horizontal:
                    if (AllowDifferentSizedItems) return;
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor(GetWidth(_viewport.Size) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex -= itemsPerRow;
                    break;
            }
        }

        /// <summary>
        /// This method is called if the user wants to navigate to the down.
        /// </summary>
        /// <param name="currentIndex">the current index. Update this parameter to set the new index.</param>
        /// <remarks>
        /// Override this method if you want to configure how navigation works. Especially useful if
        /// <see cref="AllowDifferentSizedItems"/> is <see langword="true"></see>
        /// </remarks>
        protected void NavigateDown(ref int currentIndex)
        {
            switch (Orientation)
            {
                case Orientation.Vertical:
                    ++currentIndex;
                    break;
                case Orientation.Horizontal:
                    if (AllowDifferentSizedItems) return;
                    var itemsPerRow =
                        (int)Math.Max(Math.Floor(GetWidth(_viewport.Size) / GetWidth(GetAverageItemSize())), 1);
                    currentIndex += itemsPerRow;
                    break;
            }
        }
        

        #endregion

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
        /// Prepares a container if the item is it's own container 
        /// </summary>
        /// <param name="item">the item to use</param>
        /// <param name="index">the item index</param>
        /// <returns>the prepared container</returns>
        private Control GetItemAsOwnContainer(object? item, int index)
        {
            Debug.Assert(ItemContainerGenerator is not null);

            var controlItem = (Control)item!;
            var generator = ItemContainerGenerator!;

            if (!controlItem.IsSet(RecycleKeyProperty))
            {
                generator.PrepareItemContainer(controlItem, controlItem, index);
                AddInternalChild(controlItem);
                controlItem.SetValue(RecycleKeyProperty, s_itemIsItsOwnContainer);
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

            container.SetValue(RecycleKeyProperty, recycleKey);
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

            var recycleKey = element.GetValue(RecycleKeyProperty);

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

            var recycleKey = element.GetValue(RecycleKeyProperty);

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
        public IReadOnlyList<double> GetIrregularSnapPoints(Orientation orientation,
            SnapPointsAlignment snapPointsAlignment)
        {
            var snapPoints = new List<double>();
            double lineSize = 0;

            switch (orientation)
            {
                case Orientation.Horizontal:
                    if (AreHorizontalSnapPointsRegular)
                        throw new InvalidOperationException();

                    if (Orientation == Orientation.Horizontal)
                    {
                        foreach (var child in VisualChildren)
                        {
                            double snapPoint = 0;

                            if (lineSize + child.Bounds.Height > _viewport.Size.Height && lineSize != 0)
                            {
                                lineSize = 0;
                                switch (snapPointsAlignment)
                                {
                                    case SnapPointsAlignment.Near:
                                        snapPoint = child.Bounds.Left;
                                        break;
                                    case SnapPointsAlignment.Center:
                                        snapPoint = child.Bounds.Center.X;
                                        break;
                                    case SnapPointsAlignment.Far:
                                        snapPoint = child.Bounds.Right;
                                        break;
                                }

                                snapPoints.Add(snapPoint);
                            }

                            lineSize += child.Bounds.Height;
                        }
                    }

                    break;

                case Orientation.Vertical:
                    if (AreVerticalSnapPointsRegular)
                        throw new InvalidOperationException();
                    if (Orientation == Orientation.Vertical)
                    {
                        foreach (var child in VisualChildren)
                        {
                            double snapPoint = 0;

                            if (lineSize + child.Bounds.Width > _viewport.Size.Width && lineSize != 0)
                            {
                                lineSize = 0;
                                switch (snapPointsAlignment)
                                {
                                    case SnapPointsAlignment.Near:
                                        snapPoint = child.Bounds.Top;
                                        break;
                                    case SnapPointsAlignment.Center:
                                        snapPoint = child.Bounds.Center.Y;
                                        break;
                                    case SnapPointsAlignment.Far:
                                        snapPoint = child.Bounds.Bottom;
                                        break;
                                }

                                snapPoints.Add(snapPoint);
                            }

                            lineSize += child.Bounds.Width;
                        }
                    }

                    break;
            }

            return snapPoints;
        }

        /// <inheritdoc/>
        public double GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment snapPointsAlignment,
            out double offset)
        {
            offset = 0f;
            var firstChild = VisualChildren.FirstOrDefault();

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
}
