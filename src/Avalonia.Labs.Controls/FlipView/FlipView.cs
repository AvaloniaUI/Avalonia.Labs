using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
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
                Orientation = Orientation.Horizontal
            });

        private Button? _previousButtonVertical;
        private Button? _nextButtonHorizontal;
        private Button? _previousButtonHorizontal;
        private Button? _nextButtonVertical;
        private bool _isApplied;
        private bool _isHorizontal;

        internal ItemsPresenter? ItemsPresenterPart { get; private set; }
        internal FlipViewScrollViewer? ScrollViewerPart { get; private set; }

        static FlipView()
        {
            SelectionModeProperty.OverrideDefaultValue<FlipView>(SelectionMode.AlwaysSelected);
            ItemsPanelProperty.OverrideDefaultValue<FlipView>(DefaultPanel);
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new FlipViewItem();

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<FlipViewItem>(item, out recycleKey);
        }

        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
        {
            if(element is FlipViewItem viewItem)
            {
                //viewItem.Content = null;
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

            if(_nextButtonHorizontal != null)
            {
                _nextButtonHorizontal.Click += NextButton_Click;
            }

            if(_nextButtonVertical != null)
            {
                _nextButtonVertical.Click += NextButton_Click;
            }

            if(_previousButtonHorizontal != null)
            {
                _previousButtonHorizontal.Click += PreviousButton_Click;
            }

            if(_previousButtonVertical != null)
            {
                _previousButtonVertical.Click += PreviousButton_Click;
            }

            if (ScrollViewerPart != null)
            {
                ScrollViewerPart.RemoveHandler(Gestures.ScrollGestureEndedEvent, ScrollEndedEventHandler);
            }

            ScrollViewerPart = e.NameScope.Find<FlipViewScrollViewer>("PART_ScrollViewer");

            if(ScrollViewerPart != null)
            {
                ScrollViewerPart.AddHandler(Gestures.ScrollGestureEndedEvent, ScrollEndedEventHandler, handledEventsToo: true);
            }

            _isApplied = true;

            SetButtonsVisibility();
        }

        private void ScrollEndedEventHandler(object? sender, ScrollGestureEndedEventArgs e)
        {
            UpdateSelectedIndex();
        }

        private void UpdateSelectedIndex()
        {
            if (ItemsPresenterPart != null && ScrollViewerPart != null && ItemCount > 0)
            {
                var offset = _isHorizontal ? ScrollViewerPart.Offset.X : ScrollViewerPart.Offset.Y;

                var index = offset / (long)(_isHorizontal ? ScrollViewerPart.Viewport.Width : ScrollViewerPart.Viewport.Height);

                SelectedIndex = (int)Math.Max(0, Math.Min(index, ItemCount));
            }
        }

        private void PreviousButton_Click(object? sender, RoutedEventArgs e)
        {
            if(ItemCount > 0)
            {
                SelectedIndex = Math.Max(0, SelectedIndex - 1);
            }
        }

        private void NextButton_Click(object? sender, Interactivity.RoutedEventArgs e)
        {
            if (ItemCount > 0)
            {
                SelectedIndex = Math.Min(ItemCount - 1, SelectedIndex + 1);
            }
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (ItemCount > 0)
            {
                SelectedIndex = Math.Max(0, Math.Min(ItemCount - 1, SelectedIndex + (e.Delta.Y < 0 ? 1 : - 1)));
            }
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
                        _nextButtonHorizontal!.IsVisible = true;
                        _previousButtonHorizontal!.IsVisible = true;
                        _nextButtonVertical!.IsVisible = false;
                        _previousButtonVertical!.IsVisible = false;
                        _isHorizontal = true;
                        break;
                    case Orientation.Vertical:
                        _nextButtonVertical!.IsVisible = true;
                        _previousButtonVertical!.IsVisible = true;
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
                        _nextButtonHorizontal!.IsVisible = true;
                        _previousButtonHorizontal!.IsVisible = true;
                        _nextButtonVertical!.IsVisible = false;
                        _previousButtonVertical!.IsVisible = false;
                        _isHorizontal = true;
                        break;
                    case Orientation.Vertical:
                        _nextButtonVertical!.IsVisible = true;
                        _previousButtonVertical!.IsVisible = true;
                        _nextButtonHorizontal!.IsVisible = false;
                        _previousButtonHorizontal!.IsVisible = false;
                        _isHorizontal = false;
                        break;
                }
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == ItemsPanelProperty)
            {
                SetButtonsVisibility();
            }
        }
    }
}
