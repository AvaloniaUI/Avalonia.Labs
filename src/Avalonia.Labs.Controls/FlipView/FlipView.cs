using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

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

        public static readonly StyledProperty<Vector> OffsetProperty = AvaloniaProperty.Register<FlipView, Vector>(nameof(Offset));

        private Button? _previousButtonVertical;
        private Button? _nextButtonHorizontal;
        private Button? _previousButtonHorizontal;
        private Button? _nextButtonVertical;
        private VectorTransition? _offsetTransistion;
        private bool _isApplied;
        private ImplicitAnimationCollection? _implicitAnimations;
        private bool _animationsDisabled;
        private bool _isAnimating;
        private Vector? _finalValue;

        internal ItemsPresenter? ItemsPresenterPart { get; private set; }
        internal ScrollViewer? ScrollViewerPart { get; private set; }

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

        public Vector Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            ItemsPresenterPart = e.NameScope.Get<ItemsPresenter>("PART_ItemsPresenter");

            ItemsPresenterPart.Loaded += (s, e) =>
            {
                if (ItemsPresenterPart != null)
                {
                    var composition = ElementComposition.GetElementVisual(ItemsPresenterPart);

                    if (composition != null)
                    {
                        if (_implicitAnimations == null)
                        {
                            var compositor = composition.Compositor;

                            var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
                            offsetAnimation.Target = "Offset";
                            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                            offsetAnimation.Duration = TimeSpan.FromMilliseconds(250);

                            _implicitAnimations = compositor.CreateImplicitAnimationCollection();
                            _implicitAnimations["Offset"] = offsetAnimation;
                        }

                       // composition.ImplicitAnimations = _implicitAnimations;
                    }
                }
            };

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

            var grid = e.NameScope.Find<Grid>("PART_Grid");
            ScrollViewerPart = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");

            if (ScrollViewerPart != null)
            {
                ScrollViewerPart.PropertyChanged += ScrollViewerPart_PropertyChanged;
            }

            _offsetTransistion = new VectorTransition()
            {
                Property = OffsetProperty,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            Transitions = new Transitions
            {
                _offsetTransistion
            };

            _isApplied = true;

            SetButtonsVisibility();

            if (grid != null)
            {
                grid.AddHandler(Gestures.ScrollGestureEvent, ScrollEventHandler, handledEventsToo: true);
                grid.AddHandler(Gestures.ScrollGestureEndedEvent, ScrollEndedEventHandler, handledEventsToo: true);
            }
        }

        private void ScrollViewerPart_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Property == ScrollViewer.OffsetProperty && e.NewValue != null)
            {
                var newValue = (Vector?)e.NewValue;

                if (_animationsDisabled)
                {
                    _isAnimating = false;
                    Offset = (Vector)e.NewValue;
                }
                else
                {
                    if (ItemsPresenterPart != null)
                    {
                        var composition = ElementComposition.GetElementVisual(ItemsPresenterPart);

                        if (composition != null)
                        {
                            composition.ImplicitAnimations = null;
                        }
                    }

                    if (_isAnimating)
                    {
                        if (newValue == _finalValue)
                        {
                            _isAnimating = false;
                        }
                        return;
                    }
                    _isAnimating = true;
                    _finalValue =  newValue;

                    Offset = (Vector)e.NewValue;

                    if (ScrollViewerPart != null)
                    {
                        ScrollViewerPart.Offset = (Vector?)e.OldValue ?? default;
                    }

                }
            }
        }

        private void ScrollEndedEventHandler(object? sender, ScrollGestureEndedEventArgs e)
        {
            UpdateAnimation(true);

            if (ItemsPresenterPart != null && ScrollViewerPart != null && ItemCount > 0)
            {
                var composition = ElementComposition.GetElementVisual(ItemsPresenterPart);

                if (composition != null)
                {
                    composition.ImplicitAnimations = _implicitAnimations;
                }

                bool isHorizontal = _nextButtonHorizontal!.IsVisible;

                var offset = isHorizontal ? ScrollViewerPart.Offset.X : ScrollViewerPart.Offset.Y;

                var index = offset / (long)(isHorizontal ? Bounds.Width : Bounds.Height);

                SelectedIndex = (int)Math.Max(0, index);
            }
        }

        private void ScrollEventHandler(object? sender, ScrollGestureEventArgs e)
        {
            UpdateAnimation(false);
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
                        break;
                    case Orientation.Vertical:
                        _nextButtonVertical!.IsVisible = true;
                        _previousButtonVertical!.IsVisible = true;
                        _nextButtonHorizontal!.IsVisible = false;
                        _previousButtonHorizontal!.IsVisible = false;
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
                        break;
                    case Orientation.Vertical:
                        _nextButtonVertical!.IsVisible = true;
                        _previousButtonVertical!.IsVisible = true;
                        _nextButtonHorizontal!.IsVisible = false;
                        _previousButtonHorizontal!.IsVisible = false;
                        break;
                }
            }
        }

        private void UpdateAnimation(bool enable)
        {
            if (ItemsPresenterPart != null && (!_animationsDisabled || enable))
            {
                if (_offsetTransistion != null)
                {
                    if (enable)
                    {
                        if (this.Transitions?.Contains(_offsetTransistion) == false)
                        {
                            this.Transitions?.Add(_offsetTransistion);
                        }
                    }
                    else
                    {
                        this?.Transitions?.Remove(_offsetTransistion);
                    }
                }

                _animationsDisabled = !enable;
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == ItemsPanelProperty)
            {
                SetButtonsVisibility();
            }
            else if(change.Property == OffsetProperty)
            {
                if(Offset == ScrollViewerPart?.Offset)
                {
                    _isAnimating = false;
                }
            }
        }
    }
}
