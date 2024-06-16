using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;

namespace Avalonia.Labs.Controls;

public class FlipViewScrollGestureRecognizer : ScrollGestureRecognizer
{
    public static readonly RoutedEvent<PointerPressedEventArgs> ScrollGesturePointerPressedEvent =
        RoutedEvent.Register<PointerPressedEventArgs>(
            "ScrollGesturePointerPressed", RoutingStrategies.Bubble, typeof(Gestures));
    public static readonly RoutedEvent<PointerReleasedEventArgs> ScrollGesturePointerReleasedEvent =
        RoutedEvent.Register<PointerReleasedEventArgs>(
            "ScrollGesturePointerReleased", RoutingStrategies.Bubble, typeof(Gestures));
    public static readonly RoutedEvent<PointerEventArgs> ScrollGesturePointerLostEvent =
        RoutedEvent.Register<PointerEventArgs>(
            "ScrollGesturePointerLost", RoutingStrategies.Bubble, typeof(Gestures));
    
    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        e.RoutedEvent = ScrollGesturePointerPressedEvent;
        Target?.RaiseEvent(e);
        base.PointerPressed(e);
    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        e.RoutedEvent = ScrollGesturePointerReleasedEvent;
        Target?.RaiseEvent(e);
        base.PointerReleased(e);
    }

    protected override void PointerCaptureLost(IPointer pointer)
    {
        Target?.RaiseEvent(new RoutedEventArgs(ScrollGesturePointerLostEvent));
        base.PointerCaptureLost(pointer);
    }
}
