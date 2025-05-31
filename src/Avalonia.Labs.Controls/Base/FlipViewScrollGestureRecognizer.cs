using System;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Avalonia.Labs.Controls;

internal class FlipViewSwipeEventArgs : RoutedEventArgs
{
    public FlipViewSwipeEventArgs(double swipeDirection) : base(FlipViewScrollGestureRecognizer.FlipViewSwipeEvent)
    {
        SwipeDirection = swipeDirection;
    }

    public double SwipeDirection { get; }
}

public class FlipViewScrollGestureRecognizer : GestureRecognizer
{
    internal static readonly RoutedEvent<FlipViewSwipeEventArgs> FlipViewSwipeEvent =
        RoutedEvent.Register<FlipViewSwipeEventArgs>(
            "FlipViewSwipe", RoutingStrategies.Bubble, typeof(FlipViewScrollGestureRecognizer));

    // Pixels per second speed that is considered to be the stop of inertial scroll
    internal const double InertialScrollSpeedEnd = 5;
    public const double InertialResistance = 0.15;

    private bool _canHorizontallyScroll;
    private bool _canVerticallyScroll;
    private const int s_defaultScrollStartDistance = 5;
    private int _scrollStartDistance = s_defaultScrollStartDistance;

    private bool _scrolling;
    private Point _trackedRootPoint;
    private IPointer? _tracking;
    private int _gestureId;
    private Point _pointerPressedPoint;
    private VelocityTracker? _velocityTracker;

    // Movement per second
    private Vector _inertia;
    private ulong? _lastMoveTimestamp;
    private ScrollContentPresenter? _currentTrackingScrollViewer;

    /// <summary>
    /// Defines the <see cref="CanHorizontallyScroll"/> property.
    /// </summary>
    public static readonly DirectProperty<FlipViewScrollGestureRecognizer, bool> CanHorizontallyScrollProperty =
        AvaloniaProperty.RegisterDirect<FlipViewScrollGestureRecognizer, bool>(nameof(CanHorizontallyScroll),
            o => o.CanHorizontallyScroll, (o, v) => o.CanHorizontallyScroll = v);

    /// <summary>
    /// Defines the <see cref="CanVerticallyScroll"/> property.
    /// </summary>
    public static readonly DirectProperty<FlipViewScrollGestureRecognizer, bool> CanVerticallyScrollProperty =
        AvaloniaProperty.RegisterDirect<FlipViewScrollGestureRecognizer, bool>(nameof(CanVerticallyScroll),
            o => o.CanVerticallyScroll, (o, v) => o.CanVerticallyScroll = v);

    /// <summary>
    /// Defines the <see cref="ScrollStartDistance"/> property.
    /// </summary>
    public static readonly DirectProperty<FlipViewScrollGestureRecognizer, int> ScrollStartDistanceProperty =
        AvaloniaProperty.RegisterDirect<FlipViewScrollGestureRecognizer, int>(nameof(ScrollStartDistance),
            o => o.ScrollStartDistance, (o, v) => o.ScrollStartDistance = v,
            unsetValue: s_defaultScrollStartDistance);

    /// <summary>
    /// Gets or sets a value indicating whether the content can be scrolled horizontally.
    /// </summary>
    public bool CanHorizontallyScroll
    {
        get => _canHorizontallyScroll;
        set => SetAndRaise(CanHorizontallyScrollProperty, ref _canHorizontallyScroll, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the content can be scrolled vertically.
    /// </summary>
    public bool CanVerticallyScroll
    {
        get => _canVerticallyScroll;
        set => SetAndRaise(CanVerticallyScrollProperty, ref _canVerticallyScroll, value);
    }

    /// <summary>
    /// Gets or sets a value indicating the distance the pointer moves before scrolling is started
    /// </summary>
    public int ScrollStartDistance
    {
        get => _scrollStartDistance;
        set => SetAndRaise(ScrollStartDistanceProperty, ref _scrollStartDistance, value);
    }

    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(null);

        if (e.Pointer.Type is PointerType.Touch or PointerType.Pen
            && point.Properties.IsLeftButtonPressed)
        {
            EndGesture();
            _tracking = e.Pointer;
            _gestureId = ScrollGestureEventArgs.GetNextFreeId();
            _trackedRootPoint = _pointerPressedPoint = point.Position;
            _velocityTracker = new VelocityTracker();
            _velocityTracker?.AddPosition(TimeSpan.FromMilliseconds(e.Timestamp), default);
        }
        AttachScrollViewer();
    }

    private void AttachScrollViewer()
    {
        _currentTrackingScrollViewer = Target as ScrollContentPresenter;
    }

    private void DetachScrollViewer()
    {
        _currentTrackingScrollViewer = null;
    }

    protected override void PointerMoved(PointerEventArgs e)
    {
        if (e.Pointer == _tracking)
        {
            var rootPoint = e.GetPosition(null);
            if (!_scrolling)
            {
                if (CanHorizontallyScroll && Math.Abs(_trackedRootPoint.X - rootPoint.X) > ScrollStartDistance)
                    _scrolling = true;
                if (CanVerticallyScroll && Math.Abs(_trackedRootPoint.Y - rootPoint.Y) > ScrollStartDistance)
                    _scrolling = true;
                if (_scrolling)
                {
                    // Correct _trackedRootPoint with ScrollStartDistance, so scrolling does not start with a skip of ScrollStartDistance
                    _trackedRootPoint = new Point(
                        _trackedRootPoint.X - (_trackedRootPoint.X >= rootPoint.X ? ScrollStartDistance : -ScrollStartDistance),
                        _trackedRootPoint.Y - (_trackedRootPoint.Y >= rootPoint.Y ? ScrollStartDistance : -ScrollStartDistance));

                    Capture(e.Pointer);
                }
            }

            if (_scrolling)
            {
                var vector = _trackedRootPoint - rootPoint;

                _velocityTracker?.AddPosition(TimeSpan.FromMilliseconds(e.Timestamp), _pointerPressedPoint - rootPoint);

                _lastMoveTimestamp = e.Timestamp;
                Target!.RaiseEvent(new ScrollGestureEventArgs(_gestureId, vector));
                _trackedRootPoint = rootPoint;
                e.Handled = true;
            }
        }
    }

    protected override void PointerCaptureLost(IPointer pointer)
    {
        if (pointer == _tracking)
            EndGesture();
    }

    void EndGesture()
    {
        _tracking = null;
        if (_scrolling)
        {
            _inertia = default;
            _scrolling = false;
            Target!.RaiseEvent(new ScrollGestureEndedEventArgs(_gestureId));
            _gestureId = 0;
            _lastMoveTimestamp = null;
        }
        DetachScrollViewer();

    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Pointer == _tracking && _scrolling)
        {
            EndGesture();
            _inertia = _velocityTracker?.GetFlingVelocity().PixelsPerSecond ?? Vector.Zero;
            double inertiaDirection = 0;
            Orientation orientation = Orientation.Horizontal;

            if (_currentTrackingScrollViewer?.HorizontalSnapPointsType != SnapPointsType.None ||
               _currentTrackingScrollViewer?.VerticalSnapPointsType != SnapPointsType.None)
            {
                var presenter = _currentTrackingScrollViewer?.Content as ItemsPresenter;
                if (presenter?.Panel is StackPanel stackPanel)
                {
                    orientation = stackPanel.Orientation;
                }
                else if (presenter?.Panel is VirtualizingStackPanel virtualizingStackPanel)
                {
                    orientation = virtualizingStackPanel.Orientation;
                }
            }
            inertiaDirection = orientation == Orientation.Horizontal ? _inertia.X : _inertia.Y;

            if (Math.Abs(inertiaDirection) > 1000)
                Target!.RaiseEvent(new FlipViewSwipeEventArgs(inertiaDirection));
        }
    }
}
