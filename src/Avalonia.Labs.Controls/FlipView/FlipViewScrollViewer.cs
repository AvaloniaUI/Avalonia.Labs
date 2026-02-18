using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Labs.Controls
{
    internal class FlipViewScrollViewer : ContentControl, IScrollable, IScrollAnchorProvider
    {
        /// <summary>
        /// Defines the <see cref="Offset"/> property.
        /// </summary>
        public static readonly StyledProperty<Vector> OffsetProperty =
            AvaloniaProperty.Register<FlipViewScrollViewer, Vector>(nameof(Offset), coerce: CoerceOffset);

        /// <summary>
        /// Defines the <see cref="Extent"/> property.
        /// </summary>
        public static readonly DirectProperty<FlipViewScrollViewer, Size> ExtentProperty =
            AvaloniaProperty.RegisterDirect<FlipViewScrollViewer, Size>(nameof(Extent),
                o => o.Extent);

        /// <summary>
        /// Defines the <see cref="Viewport"/> property.
        /// </summary>
        public static readonly DirectProperty<FlipViewScrollViewer, Size> ViewportProperty =
            AvaloniaProperty.RegisterDirect<FlipViewScrollViewer, Size>(nameof(Viewport),
                o => o.Viewport);

        /// <summary>
        /// Defines the <see cref="HorizontalSnapPointsType"/> property.
        /// </summary>
        public static readonly AttachedProperty<SnapPointsType> HorizontalSnapPointsTypeProperty =
            ScrollViewer.HorizontalSnapPointsTypeProperty.AddOwner<FlipViewScrollViewer>();

        /// <summary>
        /// Defines the <see cref="EnableTransition"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> EnableTransitionProperty =
            AvaloniaProperty.Register<FlipViewScrollViewer, bool>(nameof(EnableTransition), defaultValue: true);

        /// <summary>
        /// Defines the <see cref="TransitionDuration"/> property.
        /// </summary>
        public static readonly StyledProperty<TimeSpan?> TransitionDurationProperty =
            AvaloniaProperty.Register<FlipViewScrollViewer, TimeSpan?>(nameof(TransitionDuration), defaultValue: TimeSpan.FromMilliseconds(500));

        /// <summary>
        /// Defines the <see cref="VerticalSnapPointsType"/> property.
        /// </summary>
        public static readonly AttachedProperty<SnapPointsType> VerticalSnapPointsTypeProperty =
            ScrollViewer.VerticalSnapPointsTypeProperty.AddOwner<FlipViewScrollViewer>();

        /// <summary>
        /// Defines the <see cref="HorizontalSnapPointsAlignment"/> property.
        /// </summary>
        public static readonly AttachedProperty<SnapPointsAlignment> HorizontalSnapPointsAlignmentProperty =
            ScrollViewer.HorizontalSnapPointsAlignmentProperty.AddOwner<FlipViewScrollViewer>();

        /// <summary>
        /// Defines the <see cref="VerticalSnapPointsAlignment"/> property.
        /// </summary>
        public static readonly AttachedProperty<SnapPointsAlignment> VerticalSnapPointsAlignmentProperty =
            ScrollViewer.VerticalSnapPointsAlignmentProperty.AddOwner<FlipViewScrollViewer>();

        /// <summary>
        /// Defines the <see cref="HorizontalScrollBarVisibility"/> property.
        /// </summary>
        public static readonly AttachedProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
            AvaloniaProperty.RegisterAttached<FlipViewScrollViewer, Control, ScrollBarVisibility>(
                nameof(HorizontalScrollBarVisibility),
                ScrollBarVisibility.Disabled);

        /// <summary>
        /// Defines the <see cref="VerticalScrollBarVisibility"/> property.
        /// </summary>
        public static readonly AttachedProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
            AvaloniaProperty.RegisterAttached<ScrollViewer, Control, ScrollBarVisibility>(
                nameof(VerticalScrollBarVisibility),
                ScrollBarVisibility.Auto);

        private Size _extent;
        private Size _viewport;
        private CompositeDisposable? _subscriptions;
        private ScrollContentPresenter? _presenter;
        private Size _oldExtent;
        private Vector _oldOffset;
        private Size _oldViewport;
        private Compositor? _compositor;
        private ImplicitAnimationCollection? _animations;
        private Vector3KeyFrameAnimation? _offsetKeyFrameAnimation;

        /// <inheritdoc/>
        public Control? CurrentAnchor => (Presenter as IScrollAnchorProvider)?.CurrentAnchor;

        static FlipViewScrollViewer()
        {
            EnableTransitionProperty.Changed.AddClassHandler<FlipViewScrollViewer>((x, e) => x.AttachAnimations());
            TransitionDurationProperty.Changed.AddClassHandler<FlipViewScrollViewer>((x, e) => x.AttachAnimations());
        }

        public FlipViewScrollViewer()
        {
            PropertyChanged += FlipViewScrollViewer_PropertyChanged;
        }

        private void FlipViewScrollViewer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ExtentProperty)
            {
                CoerceValue(OffsetProperty);
            }
            else if (e.Property == ViewportProperty)
            {
                CoerceValue(OffsetProperty);
            }
        }

        /// <summary>
        /// Gets or sets whether transitions are enabled for <see cref="Offset"/>
        /// </summary>
        public bool EnableTransition
        {
            get => GetValue(EnableTransitionProperty);
            set => SetValue(EnableTransitionProperty, value);
        }

        /// <summary>
        /// Gets or sets the duration for <see cref="Offset"/> transitions
        /// </summary>
        public TimeSpan? TransitionDuration
        {
            get => GetValue(TransitionDurationProperty);
            set => SetValue(TransitionDurationProperty, value);
        }

        /// <summary>
        /// Gets or sets how scroll gesture reacts to the snap points along the horizontal axis.
        /// </summary>
        public SnapPointsType HorizontalSnapPointsType
        {
            get => GetValue(HorizontalSnapPointsTypeProperty);
            set => SetValue(HorizontalSnapPointsTypeProperty, value);
        }

        /// <summary>
        /// Gets or sets how scroll gesture reacts to the snap points along the vertical axis.
        /// </summary>
        public SnapPointsType VerticalSnapPointsType
        {
            get => GetValue(VerticalSnapPointsTypeProperty);
            set => SetValue(VerticalSnapPointsTypeProperty, value);
        }

        /// <summary>
        /// Gets or sets how the existing snap points are horizontally aligned versus the initial viewport.
        /// </summary>
        public SnapPointsAlignment HorizontalSnapPointsAlignment
        {
            get => GetValue(HorizontalSnapPointsAlignmentProperty);
            set => SetValue(HorizontalSnapPointsAlignmentProperty, value);
        }

        /// <summary>
        /// Gets or sets how the existing snap points are vertically aligned versus the initial viewport.
        /// </summary>
        public SnapPointsAlignment VerticalSnapPointsAlignment
        {
            get => GetValue(VerticalSnapPointsAlignmentProperty);
            set => SetValue(VerticalSnapPointsAlignmentProperty, value);
        }


        /// <summary>
        /// Gets or sets the current scroll offset.
        /// </summary>
        public Vector Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        /// <summary>
        /// Gets the extent of the scrollable content.
        /// </summary>
        public Size Extent
        {
            get => _extent;

            internal set
            {
                SetAndRaise(ExtentProperty, ref _extent, value);
            }
        }

        /// <summary>
        /// Gets the size of the viewport on the scrollable content.
        /// </summary>
        public Size Viewport
        {
            get => _viewport;

            internal set
            {
                SetAndRaise(ViewportProperty, ref _viewport, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal scrollbar visibility.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets the vertical scrollbar visibility.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether the viewer can scroll horizontally.
        /// </summary>
        protected bool CanHorizontallyScroll
        {
            get => HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
        }

        bool IScrollable.CanHorizontallyScroll => CanHorizontallyScroll;

        /// <summary>
        /// Gets a value indicating whether the viewer can scroll vertically.
        /// </summary>
        protected bool CanVerticallyScroll
        {
            get => VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
        }

        bool IScrollable.CanVerticallyScroll => CanVerticallyScroll;

        /// <inheritdoc/>
        public void RegisterAnchorCandidate(Control element)
        {
            (Presenter as IScrollAnchorProvider)?.RegisterAnchorCandidate(element);
        }

        /// <inheritdoc/>
        public void UnregisterAnchorCandidate(Control element)
        {
            (Presenter as IScrollAnchorProvider)?.UnregisterAnchorCandidate(element);
        }

        /// <summary>
        /// Called when a change in scrolling state is detected, such as a change in scroll
        /// position, extent, or viewport size.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <remarks>
        /// If you override this method, call `base.OnScrollChanged(ScrollChangedEventArgs)` to
        /// ensure that this event is raised.
        /// </remarks>
        protected virtual void OnScrollChanged(ScrollChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (_presenter != null)
            {
                _subscriptions?.Dispose();
                _subscriptions = null;
            }

            _presenter = e.NameScope.Find<ScrollContentPresenter>("PART_ContentPresenter");
            if (_presenter is { } scrollContentPresenter)
            {
                AttachPresenter(scrollContentPresenter);
                _presenter.PropertyChanged += Presenter_PropertyChanged;
            }

            AttachAnimations();
        }

        private void Presenter_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_presenter == null)
            {
                return;
            }
            if (e.Property == ScrollContentPresenter.OffsetProperty && e.Priority != Data.BindingPriority.Animation)
            {
                SetCurrentValue(OffsetProperty, e.GetNewValue<Vector>());
                RaiseScrollChanged();
            }
            else if (e.Property == ScrollContentPresenter.ExtentProperty)
            {
                Extent = e.GetNewValue<Size>();
                _presenter.CoerceValue(ScrollContentPresenter.OffsetProperty);
                RaiseScrollChanged();
            }
            else if (e.Property == ScrollContentPresenter.ViewportProperty)
            {
                Viewport = e.GetNewValue<Size>();
                _presenter.CoerceValue(ScrollContentPresenter.OffsetProperty);
                RaiseScrollChanged();
            }
            else if (e.Property == ContentControl.ContentProperty)
            {
                if (_compositor != null && e.OldValue is Control oldControl)
                {
                    DetachAnimations(oldControl);
                }

                AttachAnimations();
            }
        }

        internal static Vector CoerceOffset(AvaloniaObject sender, Vector value)
        {
            var extent = sender.GetValue(ExtentProperty);
            var viewport = sender.GetValue(ViewportProperty);

            var maxX = Math.Max(extent.Width - viewport.Width, 0);
            var maxY = Math.Max(extent.Height - viewport.Height, 0);
            return new Vector(Clamp(value.X, 0, maxX), Clamp(value.Y, 0, maxY));
        }

        private static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void DetachAnimations(Control control)
        {
            if (_offsetKeyFrameAnimation != null)
            {
                var visual = ElementComposition.GetElementVisual(control);
                if (visual != null)
                {
                    visual.ImplicitAnimations = null;
                }
            }

            _offsetKeyFrameAnimation = null;
            _animations = null;
        }

        private void AttachAnimations()
        {
            if (_presenter != null)
                if (EnableTransition && TransitionDuration is { } timeSpan && _compositor != null)
                {
                    if (_animations == null)
                    {
                        _animations = _compositor.CreateImplicitAnimationCollection();

                        _offsetKeyFrameAnimation = _compositor.CreateVector3KeyFrameAnimation();
                        _offsetKeyFrameAnimation.Target = "Offset";
                        _offsetKeyFrameAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                        _offsetKeyFrameAnimation.Duration = timeSpan;

                        _animations["Offset"] = _offsetKeyFrameAnimation;
                    }

                    if (Content is Control control)
                    {
                        var visual = ElementComposition.GetElementVisual(control);
                        if (visual != null)
                        {
                            visual.ImplicitAnimations = _animations;
                        }
                    }
                }
                else
                {
                    if (Content is Control control)
                    {
                        var visual = ElementComposition.GetElementVisual(control);
                        if (visual != null)
                        {
                            visual.ImplicitAnimations = null;
                        }
                    }
                }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.Pointer.Type is PointerType.Pen or PointerType.Touch)
                DisableAnimation();

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (e.Pointer.Type is PointerType.Pen or PointerType.Touch)
                EnableAnimation();

            base.OnPointerReleased(e);
        }

        internal void DisableAnimation()
        {
            if (Content is Control control)
            {
                var visual = ElementComposition.GetElementVisual(control);
                if (visual != null)
                {
                    visual.ImplicitAnimations = null;
                }
            }
        }

        internal void EnableAnimation()
        {
            if (Content is Control control)
            {
                var visual = ElementComposition.GetElementVisual(control);
                if (visual != null)
                {
                    visual.ImplicitAnimations = _animations;
                }
            }
        }

        private void AttachPresenter(ScrollContentPresenter presenter)
        {
            _subscriptions?.Dispose();
            _compositor = ElementComposition.GetElementVisual(this)?.Compositor;

            var subscriptionDisposables = new IDisposable?[]
            {
                IfUnset(ScrollContentPresenter.OffsetProperty, p => presenter.Bind(p, this.GetBindingObservable(FlipViewScrollViewer.OffsetProperty), Data.BindingPriority.Template)),
                IfUnset(ContentProperty, p => presenter.Bind(p, this.GetBindingObservable(ContentProperty), Data.BindingPriority.Template)),
            }.Where(d => d != null).Cast<IDisposable>().ToArray();

            _subscriptions = new CompositeDisposable(subscriptionDisposables.Length);

            foreach (var disposable in subscriptionDisposables)
            {
                _subscriptions.Add(disposable);
            }

            presenter.SetCurrentValue(ScrollContentPresenter.OffsetProperty, Offset);
            presenter.SetCurrentValue(ContentPresenter.ContentProperty, Content);
            presenter.SetCurrentValue(ScrollContentPresenter.CanHorizontallyScrollProperty, true);
            presenter.SetCurrentValue(ScrollContentPresenter.CanVerticallyScrollProperty, true);

            presenter.AddHandler(Gestures.ScrollGestureEvent, OnScrollGesture);
            presenter.AddHandler(Gestures.ScrollGestureEndedEvent, OnScrollGestureEnded);

            IDisposable? IfUnset<T>(T property, Func<T, IDisposable> func) where T : AvaloniaProperty => presenter.IsSet(property) ? null : func(property);
        }

        private void OnScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
        {
            EnableAnimation();
        }

        private void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
        {
            DisableAnimation();
        }

        private void RaiseScrollChanged()
        {
            var extentDelta = new Vector(Extent.Width - _oldExtent.Width, Extent.Height - _oldExtent.Height);
            var offsetDelta = Offset - _oldOffset;
            var viewportDelta = new Vector(Viewport.Width - _oldViewport.Width, Viewport.Height - _oldViewport.Height);

            if (!extentDelta.NearlyEquals(default) || !offsetDelta.NearlyEquals(default) || !viewportDelta.NearlyEquals(default))
            {
                var e = new ScrollChangedEventArgs(extentDelta, offsetDelta, viewportDelta);
                OnScrollChanged(e);

                _oldExtent = Extent;
                _oldOffset = Offset;
                _oldViewport = Viewport;
            }
        }
    }
}
