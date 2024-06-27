using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls.Base;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls;

public class FlipViewScrollGestureRecognizer : GestureRecognizer
{
    public static readonly RoutedEvent<PointerPressedEventArgs> ScrollGesturePointerPressedEvent =
        RoutedEvent.Register<PointerPressedEventArgs>(
            "ScrollGesturePointerPressed", RoutingStrategies.Bubble, typeof(FlipViewScrollGestureRecognizer));
    public static readonly RoutedEvent<PointerReleasedEventArgs> ScrollGesturePointerReleasedEvent =
        RoutedEvent.Register<PointerReleasedEventArgs>(
            "ScrollGesturePointerReleased", RoutingStrategies.Bubble, typeof(FlipViewScrollGestureRecognizer));
    public static readonly RoutedEvent<PointerEventArgs> ScrollGesturePointerLostEvent =
        RoutedEvent.Register<PointerEventArgs>(
            "ScrollGesturePointerLost", RoutingStrategies.Bubble, typeof(FlipViewScrollGestureRecognizer));

    /*public static readonly RoutedEvent<ScrollFlickedEventArgs> ScrollFlickedEvent =
        RoutedEvent.Register<ScrollFlickedEventArgs>(
            "ScrollFlicked", RoutingStrategies.Bubble, typeof(IInputElement));*/

    // Pixels per second speed that is considered to be the stop of inertial scroll
    internal const double InertialScrollSpeedEnd = 5;
    public const double InertialResistance = 0.15;

    private bool _canHorizontallyScroll;
    private bool _canVerticallyScroll;
    private int _scrollStartDistance = 5;

    private bool _scrolling;
    private Point _trackedRootPoint;
    private IPointer? _tracking;
    private int _gestureId;
    private Point _pointerPressedPoint;
    private Visual? _rootTarget;

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
            unsetValue: 5);

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
        e.RoutedEvent = ScrollGesturePointerPressedEvent;
        Target?.RaiseEvent(e);

        if (e.Pointer.Type == PointerType.Touch || e.Pointer.Type == PointerType.Pen)
        {
            EndGesture();
            _tracking = e.Pointer;
            _gestureId = ScrollGestureEventArgs.GetNextFreeId();
            _rootTarget = TopLevel.GetTopLevel(Target as Visual);
            _trackedRootPoint = _pointerPressedPoint = e.GetPosition(_rootTarget);
        }
    }

    protected override void PointerMoved(PointerEventArgs e)
    {
        if (e.Pointer == _tracking)
        {
            var rootPoint = e.GetPosition(_rootTarget);
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
                Target!.RaiseEvent(new ScrollGestureEventArgs(_gestureId, vector));
                _trackedRootPoint = rootPoint;
                e.Handled = true;
            }
        }
    }

    protected override void PointerCaptureLost(IPointer pointer)
    {
        Target?.RaiseEvent(new RoutedEventArgs(ScrollGesturePointerLostEvent));

        if (pointer == _tracking)
            EndGesture();
    }

    void EndGesture(bool setHandled = false)
    {
        _tracking = null;
        if (_scrolling)
        {
            _scrolling = false;
            Target!.RaiseEvent(new ScrollGestureEndedEventArgs(_gestureId)
            {
                Handled = setHandled
            });
            _gestureId = 0;
            _rootTarget = null;
        }

    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        e.RoutedEvent = ScrollGesturePointerReleasedEvent;
        Target?.RaiseEvent(e);

        if (e.Pointer == _tracking && _scrolling)
        {
            e.Handled = true;
            EndGesture(true);
        }
    }
}
