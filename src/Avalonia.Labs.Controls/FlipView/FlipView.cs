using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls.Base;
using Avalonia.Layout;

namespace Avalonia.Labs.Controls
{
    [TemplatePart("PART_PreviousButtonHorizontal", typeof(Button))]
    [TemplatePart("PART_NextButtonHorizontal", typeof(Button))]
    [TemplatePart("PART_PreviousButtonVertical", typeof(Button))]
    [TemplatePart("PART_NextButtonVertical", typeof(Button))]
    public class FlipView : SelectingItemsControl
    {
        private static readonly FuncTemplate<Panel?> DefaultPanel =
            new FuncTemplate<Panel?>(() => new StackPanel()
            {
                Orientation = Orientation.Vertical
            });

        private Button? _previousButtonVertical;
        private Button? _nextButtonHorizontal;
        private Button? _previousButtonHorizontal;
        private Button? _nextButtonVertical;
        private bool _isApplied;
        private bool _isHorizontal;

        /// <summary>
        /// Defines the <see cref="IsButtonsVisible"/> property.
        /// </summary>
        public static StyledProperty<bool> IsButtonsVisibleProperty = AvaloniaProperty.Register<FlipView, bool>(nameof(IsButtonsVisible), defaultValue: true);

        /// <summary>
        /// Defines the <see cref="TransitionDuration"/> property.
        /// </summary>
        public static readonly StyledProperty<TimeSpan?> TransitionDurationProperty =
            FlipViewScrollViewer.TransitionDurationProperty.AddOwner<FlipView>();

        private bool _arranged;

        internal ItemsPresenter? ItemsPresenterPart { get; private set; }
        internal FlipViewScrollViewer? ScrollViewerPart { get; private set; }

        /// <summary>
        /// Gets or sets whether navigation buttons are visible on the flipview.
        /// </summary>
        public bool IsButtonsVisible
        {
            get => GetValue(IsButtonsVisibleProperty);
            set => SetValue(IsButtonsVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets the duration of transitions for the flipview.
        /// </summary>
        public TimeSpan? TransitionDuration
        {
            get => GetValue(TransitionDurationProperty);
            set => SetValue(TransitionDurationProperty, value);
        }

        static FlipView()
        {
            SelectionModeProperty.OverrideDefaultValue<FlipView>(SelectionMode.AlwaysSelected);
            ItemsPanelProperty.OverrideDefaultValue<FlipView>(DefaultPanel);
            AutoScrollToSelectedItemProperty.OverrideDefaultValue<FlipView>(false);
        }

        public FlipView()
        {
            AddHandler(PointerWheelChangedEvent, FlipPointerWheelChanged, handledEventsToo: true);
            AddHandler(KeyDownEvent, FlipKeyDown, handledEventsToo: true);
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new FlipViewItem();

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<FlipViewItem>(item, out recycleKey);
        }

        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
        {
            if (element is FlipViewItem viewItem)
            {
                viewItem.Content = item;
                element.Width = GetDesiredItemWidth();
                element.Height = GetDesiredItemHeight();
            }
            base.PrepareContainerForItemOverride(element, item, index);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            ItemsPresenterPart = e.NameScope.Get<ItemsPresenter>("PART_ItemsPresenter");

            _nextButtonHorizontal = e.NameScope.Get<Button>("PART_NextButtonHorizontal");
            _previousButtonHorizontal = e.NameScope.Get<Button>("PART_PreviousButtonHorizontal");
            _nextButtonVertical = e.NameScope.Get<Button>("PART_NextButtonVertical");
            _previousButtonVertical = e.NameScope.Get<Button>("PART_PreviousButtonVertical");

            if (_nextButtonHorizontal != null)
            {
                _nextButtonHorizontal.Click += NextButton_Click;
            }

            if (_nextButtonVertical != null)
            {
                _nextButtonVertical.Click += NextButton_Click;
            }

            if (_previousButtonHorizontal != null)
            {
                _previousButtonHorizontal.Click += PreviousButton_Click;
            }

            if (_previousButtonVertical != null)
            {
                _previousButtonVertical.Click += PreviousButton_Click;
            }

            if (ScrollViewerPart != null)
            {
                ScrollViewerPart.RemoveHandler(Gestures.ScrollGestureEndedEvent, ScrollEndedEventHandler);
                ScrollViewerPart.SizeChanged -= ScrollViewerPart_SizeChanged;
            }

            ScrollViewerPart = e.NameScope.Find<FlipViewScrollViewer>("PART_ScrollViewer");

            if (ScrollViewerPart != null)
            {
                ScrollViewerPart.AddHandler(Gestures.ScrollGestureEndedEvent, ScrollEndedEventHandler, handledEventsToo: true);
                ScrollViewerPart.SizeChanged += ScrollViewerPart_SizeChanged;
            }

            _isApplied = true;

            SetButtonsVisibility();
        }

        private void ScrollViewerPart_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            SetItemSize();

            if (ScrollViewerPart != null)
            {
                var enableTransition = ScrollViewerPart.EnableTransition;
                ScrollViewerPart.EnableTransition = false;
                this.ScrollIntoView(SelectedIndex);
                ScrollViewerPart.EnableTransition = enableTransition;
            }
        }

        private void SetItemSize()
        {
            var width = GetDesiredItemWidth();
            var height = GetDesiredItemHeight();

            var item = ContainerFromIndex(SelectedIndex);
            if (item is FlipViewItem flipViewItem)
            {
                flipViewItem.Width = width;
                flipViewItem.Height = height;
            }
        }

        private void ScrollEndedEventHandler(object? sender, ScrollGestureEndedEventArgs e)
        {
            UpdateSelectedIndex();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var arrange = base.ArrangeOverride(finalSize);

            if (!_arranged)
            {
                var width = GetDesiredItemWidth();
                var height = GetDesiredItemHeight();

                for (var i = 0; i < ItemCount; i++)
                {
                    var item = ContainerFromIndex(i);
                    if (item is FlipViewItem flipViewItem)
                    {
                        flipViewItem.Width = width;
                        flipViewItem.Height = height;
                    }
                }
            }

            _arranged = true;

            return arrange;
        }

        private void UpdateSelectedIndex()
        {
            if (ItemsPresenterPart != null && ScrollViewerPart != null && ItemCount > 0)
            {
                var offset = _isHorizontal ? ScrollViewerPart.Offset.X : ScrollViewerPart.Offset.Y;
                var viewport =_isHorizontal ? ScrollViewerPart.Viewport.Width : ScrollViewerPart.Viewport.Height;
                var viewPortIndex = (long)(offset / viewport);
                var lowerBounds = viewPortIndex * viewport;
                var midPoint = lowerBounds + (viewport * 0.5);

                var index = offset > midPoint ? viewPortIndex + 1 : viewPortIndex;

                SetScrollViewerOffset((int)Math.Max(0, Math.Min(index, ItemCount)));
            }
        }

        private void PreviousButton_Click(object? sender, RoutedEventArgs e)
        {
            MovePrevious();
        }

        private void NextButton_Click(object? sender, Interactivity.RoutedEventArgs e)
        {
            MoveNext();
        }

        protected void FlipPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y < 0)
            {
                MoveNext();
            }
            else
            {
                MovePrevious();
            }
        }

        private void MoveNext()
        {
            if (ItemCount > 0)
            {
                SetScrollViewerOffset(Math.Min(ItemCount - 1, SelectedIndex + 1));
            }
        }

        private void MovePrevious()
        {
            if (ItemCount > 0)
            {
                SetScrollViewerOffset(Math.Max(0, SelectedIndex - 1));
            }
        }

        private void MoveStart()
        {
            if (ItemCount > 0)
            {
                SetScrollViewerOffset(0);
            }
        }

        private void MoveEnd()
        {
            if (ItemCount > 0)
            {
                SetScrollViewerOffset(Math.Max(0, ItemCount - 1));
            }
        }

        private void FlipKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Left:
                case Key.PageUp:
                    MovePrevious();
                    break;

                case Key.Down:
                case Key.Right:
                case Key.PageDown:
                    MoveNext();
                    break;
                case Key.Home:
                    MoveStart();
                    break;
                case Key.End:
                    MoveEnd();
                    break;
            }
        }

        internal double GetDesiredItemWidth()
        {
            double width = 0;
            if (ItemsPresenterPart is { } presenter)
            {
                if (presenter.Panel is VirtualizingPanel virtualizingPanel)
                {
                    width = virtualizingPanel.Bounds.Width;
                }

                if (width == 0)
                {
                    width = ScrollViewerPart != null ? ScrollViewerPart.Bounds.Width : Bounds.Width;
                }
            }

            if (width == 0)
            {
                width = Width;
            }

            return width;
        }

        internal double GetDesiredItemHeight()
        {
            double height = 0;
            if (ItemsPresenterPart is { } presenter)
            {
                if (presenter.Panel is VirtualizingPanel virtualizingPanel)
                {
                    height = virtualizingPanel.Bounds.Height;
                }

                if (height == 0)
                {
                    height = ScrollViewerPart != null ? ScrollViewerPart.Bounds.Height : Bounds.Height;
                }
            }

            if (height == 0)
            {
                height = Height;
            }

            return height;
        }

        private void SetButtonsVisibility()
        {
            if (!_isApplied)
            {
                return;
            }

            var panel = ItemsPanel.Build();

            if (panel is StackPanel stackPanel)
            {
                switch (stackPanel.Orientation)
                {
                    case Orientation.Horizontal:
                        _nextButtonHorizontal!.IsVisible = true && IsButtonsVisible;
                        _previousButtonHorizontal!.IsVisible = true && IsButtonsVisible;
                        _nextButtonVertical!.IsVisible = false;
                        _previousButtonVertical!.IsVisible = false;
                        _isHorizontal = true;
                        break;
                    case Orientation.Vertical:
                        _nextButtonVertical!.IsVisible = true && IsButtonsVisible;
                        _previousButtonVertical!.IsVisible = true && IsButtonsVisible;
                        _nextButtonHorizontal!.IsVisible = false;
                        _previousButtonHorizontal!.IsVisible = false;
                        _isHorizontal = false;
                        break;
                }
            }

            if (panel is VirtualizingStackPanel virtualizingStackPanel)
            {
                switch (virtualizingStackPanel.Orientation)
                {
                    case Orientation.Horizontal:
                        _nextButtonHorizontal!.IsVisible = true && IsButtonsVisible;
                        _previousButtonHorizontal!.IsVisible = true && IsButtonsVisible;
                        _nextButtonVertical!.IsVisible = false;
                        _previousButtonVertical!.IsVisible = false;
                        _isHorizontal = true;
                        break;
                    case Orientation.Vertical:
                        _nextButtonVertical!.IsVisible = true && IsButtonsVisible;
                        _previousButtonVertical!.IsVisible = true && IsButtonsVisible;
                        _nextButtonHorizontal!.IsVisible = false;
                        _previousButtonHorizontal!.IsVisible = false;
                        _isHorizontal = false;
                        break;
                }
            }
        }

        protected Vector IndexToOffset(int index)
        {
            var container = ContainerFromIndex(index);
            var panel = ItemsPanelRoot;
            var scrollViewer = ScrollViewerPart;
            if (container == null || panel == null || scrollViewer == null)
                return default;

            var bounds = container.Bounds;
            var offset = scrollViewer.Offset;

            if (bounds.Bottom > offset.Y + scrollViewer.Viewport.Height)
            {
                offset = offset.WithY((bounds.Bottom - scrollViewer.Viewport.Height) + panel.Margin.Top);
            }

            if (bounds.Y < offset.Y)
            {
                offset = offset.WithY(bounds.Y);
            }

            if (bounds.Right > offset.X + scrollViewer.Viewport.Width)
            {
                offset = offset.WithX((bounds.Right - scrollViewer.Viewport.Width) + panel.Margin.Left);
            }

            if (bounds.X < offset.X)
            {
                offset = offset.WithX(bounds.X);
            }

            return offset;
        }

        private void SetScrollViewerOffset(int index)
        {
            var offset = IndexToOffset(index);
            SetCurrentValue(SelectedIndexProperty, index);

            if (ScrollViewerPart is { } scrollViewer)
            {
                scrollViewer.SetCurrentValue(FlipViewScrollViewer.OffsetProperty, offset);
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedIndexProperty)
            {
                SetScrollViewerOffset(change.GetNewValue<int>());
            }

            if (change.Property == ItemsPanelProperty || change.Property == IsButtonsVisibleProperty)
            {
                SetButtonsVisibility();
            }
        }
    }
}
