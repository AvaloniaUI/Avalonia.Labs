using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// A tab control that displays a tab strip along with the content of the selected tab.
    /// </summary>
    [TemplatePart("PART_Header", typeof(TabHeader))]
    [TemplatePart("PART_Border", typeof(Border))]
    [TemplatePart("PART_ScrollViewer", typeof(AnimatedScrollViewer))]
    [TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
    [PseudoClasses(":animate")]
    public class TabControl : SelectingItemsControl
    {
        /// <summary>
        /// Defines the <see cref="TabStripPlacement"/> property.
        /// </summary>
        public static readonly StyledProperty<Dock> TabStripPlacementProperty =
            Avalonia.Controls.TabControl.TabStripPlacementProperty.AddOwner<TabControl>();

        /// <summary>
        /// Defines the <see cref="HorizontalContentAlignment"/> property.
        /// </summary>
        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            ContentControl.HorizontalContentAlignmentProperty.AddOwner<TabControl>();

        /// <summary>
        /// Defines the <see cref="VerticalContentAlignment"/> property.
        /// </summary>
        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            ContentControl.VerticalContentAlignmentProperty.AddOwner<TabControl>();

        /// <summary>
        /// Defines the <see cref="HeaderDisplayMemberBinding" /> property
        /// </summary>
        public static readonly StyledProperty<IBinding?> HeaderDisplayMemberBindingProperty =
            AvaloniaProperty.Register<ItemsControl, IBinding?>(nameof(HeaderDisplayMemberBinding));

        /// <summary>
        /// The default value for the <see cref="HeaderPanel"/> property.
        /// </summary>
        private static readonly FuncTemplate<Panel> DefaultHeaderPanel =
            new FuncTemplate<Panel>(() => new StackPanel());

        /// <summary>
        /// Defines the <see cref="HeaderPanel"/> property.
        /// </summary>
        public static readonly StyledProperty<ITemplate<Panel>> HeaderPanelProperty =
            AvaloniaProperty.Register<TabControl, ITemplate<Panel>>(nameof(HeaderPanel), DefaultHeaderPanel);

        /// <summary>
        /// The default value for the <see cref="ItemsControl.ItemsPanel"/> property.
        /// </summary>
        private static readonly FuncTemplate<Panel?> DefaultPanel =
            new FuncTemplate<Panel?>(() => new VirtualizingStackPanel()
            {
                Orientation = Orientation.Horizontal
            });

        /// <summary>
        /// Defines the <see cref="HeaderTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
            AvaloniaProperty.Register<HeaderedContentControl, IDataTemplate?>(nameof(HeaderTemplate));

        /// <summary>
        /// Initializes static members of the <see cref="TabControl"/> class.
        /// </summary>
        static TabControl()
        {
            SelectionModeProperty.OverrideDefaultValue<TabControl>(SelectionMode.AlwaysSelected);
            ItemsPanelProperty.OverrideDefaultValue<TabControl>(DefaultPanel);
            AffectsMeasure<TabControl>(TabStripPlacementProperty);
            SelectedItemProperty.Changed.AddClassHandler<TabControl>((x, e) => x.UpdateHeaderSelection());
            AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<TabControl>(AutomationControlType.Tab);
        }

        public TabControl()
        {
            ItemsView.CollectionChanged += ItemsView_CollectionChanged;
        }

        private void ItemsView_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SetHeaderItems();
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the content within the control.
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the content within the control.
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get { return GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="TabHeader"/> placement of the TabControl.
        /// </summary>
        public Dock TabStripPlacement
        {
            get { return GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="IBinding"/> to use for binding to the header member of each item.
        /// </summary>
        [AssignBinding]
        public IBinding? HeaderDisplayMemberBinding
        {
            get { return GetValue(HeaderDisplayMemberBindingProperty); }
            set { SetValue(HeaderDisplayMemberBindingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data template used to display the header content of the control.
        /// </summary>
        public IDataTemplate? HeaderTemplate
        {
            get => GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the panel used to display the header items.
        /// </summary>
        public ITemplate<Panel> HeaderPanel
        {
            get { return GetValue(HeaderPanelProperty); }
            set { SetValue(HeaderPanelProperty, value); }
        }

        internal TabHeader? HeaderPart { get; private set; }
        internal ItemsPresenter? ItemsPresenterPart { get; private set; }
        internal Border? BorderPart { get; private set; }
        internal AnimatedScrollViewer? ScrollViewerPart { get; private set; }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new TabItem();

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<TabItem>(item, out recycleKey);
        }

        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
        {
            base.PrepareContainerForItemOverride(element, item, index);

            if (element is TabItem pivotItem)
            {
                if (ItemTemplate is { } it)
                    pivotItem.ContentTemplate = it;

                pivotItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                pivotItem.VerticalAlignment = VerticalAlignment.Stretch;
            }

            if (index == SelectedIndex && element is ContentControl)
            {
                UpdateHeaderSelection();
            }
        }

        protected override void ClearContainerForItemOverride(Control element)
        {
            base.ClearContainerForItemOverride(element);

            if (element is TabItem pivotItem)
            {
                pivotItem.Content = null;
            }
            UpdateHeaderSelection();
        }

        private void UpdateHeaderSelection()
        {
            if (HeaderPart != null)
            {
                HeaderPart.SelectedIndex = SelectedIndex;
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            HeaderPart = e.NameScope.Get<TabHeader>("PART_Header");
            ItemsPresenterPart = e.NameScope.Get<ItemsPresenter>("PART_ItemsPresenter");

            if (HeaderPart != null)
            {
                HeaderPart.AddHandler(SelectionChangedEvent, (o, e) => SelectedIndex = HeaderPart.SelectedIndex);

                HeaderPart.SetValue(TabHeader.TabStripPlacementProperty, TabStripPlacement);

                HeaderPart.DisplayMemberBinding = HeaderDisplayMemberBinding;

                SetHeaderItems();
            }

            BorderPart = e.NameScope.Find<Border>("PART_Border");
            ScrollViewerPart = e.NameScope.Find<AnimatedScrollViewer>("PART_ScrollViewer");

            if (ScrollViewerPart != null)
            {
                ScrollViewerPart.PropertyChanged += ScrollViewerPart_PropertyChanged;
                ScrollViewerPart.AddHandler(Gestures.ScrollGestureEvent, ScrollEventHandler, handledEventsToo: true);
                ScrollViewerPart.AddHandler(Gestures.ScrollGestureEndedEvent, ScrollEndedEventHandler, handledEventsToo: true);
            }
        }

        private void ScrollEventHandler(object? sender, ScrollGestureEventArgs e)
        {
           // PseudoClasses.Remove(":animate");
        }

        private void ScrollViewerPart_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ScrollViewer.OffsetProperty && e.NewValue != null)
            {
                Debug.WriteLine(e.NewValue);
            }
        }

        private void ScrollEndedEventHandler(object? sender, ScrollGestureEndedEventArgs e)
        {
            PseudoClasses.Add(":animate");
            if (ItemsPresenterPart != null)
            {
                if (ScrollViewerPart != null && BorderPart != null)
                {
                    var xOffset = ScrollViewerPart.Offset.X;

                    var index = xOffset / (long)BorderPart.Bounds.Width;

                    SelectedIndex = (int)Math.Max(0, index);
                }
            }
        }

        protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
        {
            base.ContainerIndexChangedOverride(container, oldIndex, newIndex);

            var selectedIndex = SelectedIndex;

            if (selectedIndex == oldIndex || selectedIndex == newIndex)
                UpdateHeaderSelection();
        }

        /// <inheritdoc/>
        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);

            if (e.NavigationMethod == NavigationMethod.Directional)
            {
                e.Handled = UpdateSelectionFromEventSource(e.Source);
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Pointer.Type == PointerType.Mouse)
            {
                e.Handled = UpdateSelectionFromEventSource(e.Source);
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            EnsureSelectionInView();

            return size;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedIndexProperty)
            {
                EnsureSelectionInView();
            }
            else if (change.Property == HeaderDisplayMemberBindingProperty)
            {
                if (HeaderPart != null)
                    HeaderPart.DisplayMemberBinding = HeaderDisplayMemberBinding;
            }
            else if (change.Property == TabStripPlacementProperty)
            {
                HeaderPart?.SetValue(TabHeader.TabStripPlacementProperty, TabStripPlacement);
            }
        }

        private void EnsureSelectionInView()
        {
            if (SelectedIndex > -1)
            {
                ScrollIntoView(SelectedIndex);
            }
        }

        private void SetHeaderItems()
        {
            if (HeaderPart != null)
            {
                List<object?> headers = new List<object?>();

                if (ItemsView == null)
                {
                    return;
                }

                foreach (var item in ItemsView)
                {
                    if (item is HeaderedContentControl headered)
                    {
                        (headered.Parent?.GetLogicalChildren() as IList<ILogical>)?.Remove(headered);
                        (headered.GetVisualParent()?.GetLogicalChildren() as IList<ILogical>)?.Remove(headered);

                        var header = new TabHeaderItem()
                        {
                            Content = headered.Header,
                        };

                        header.ContentTemplate = headered.HeaderTemplate;

                        if (headered is TabItem tabItem && tabItem.HeaderTheme != null)
                        {
                            header.Theme = tabItem.HeaderTheme;
                        }

                        headers.Add(header);
                    }
                    else
                    {
                        if (HeaderTemplate != null)
                        {
                            headers.Add(HeaderTemplate.Build(item));
                        }
                        else
                        {
                            headers.Add(item?.ToString());
                        }
                    }
                }

                HeaderPart.ItemsSource = headers;

                UpdateHeaderSelection();
            }
        }
    }
}
