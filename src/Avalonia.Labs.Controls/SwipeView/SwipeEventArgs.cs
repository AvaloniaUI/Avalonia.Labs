using Avalonia.Interactivity;

namespace Avalonia.Labs.Controls;

/// <summary>
/// Provides data for the OpenRequested event
/// </summary>
public class OpenRequestedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the direction in which the swipe is being opened
    /// </summary>
    public OpenSwipeItem OpenSwipeItem { get; }

    /// <summary>
    /// Gets or sets whether the open request should be cancelled
    /// </summary>
    public bool Cancel { get; set; }

    public OpenRequestedEventArgs(OpenSwipeItem openSwipeItem)
    {
        OpenSwipeItem = openSwipeItem;
    }

    public OpenRequestedEventArgs(RoutedEvent? routedEvent, OpenSwipeItem openSwipeItem) : base(routedEvent)
    {
        OpenSwipeItem = openSwipeItem;
    }
}

/// <summary>
/// Provides data for the CloseRequested event
/// </summary>
public class CloseRequestedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets or sets whether the close request should be cancelled
    /// </summary>
    public bool Cancel { get; set; }

    public CloseRequestedEventArgs()
    {
    }

    public CloseRequestedEventArgs(RoutedEvent? routedEvent) : base(routedEvent)
    {
    }
}

/// <summary>
/// Provides data for the SwipeStarted event
/// </summary>
public class SwipeStartedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the direction of the swipe gesture
    /// </summary>
    public SwipeDirection SwipeDirection { get; }

    public SwipeStartedEventArgs(SwipeDirection swipeDirection)
    {
        SwipeDirection = swipeDirection;
    }

    public SwipeStartedEventArgs(RoutedEvent? routedEvent, SwipeDirection swipeDirection) : base(routedEvent)
    {
        SwipeDirection = swipeDirection;
    }
}

/// <summary>
/// Provides data for the SwipeEnded event
/// </summary>
public class SwipeEndedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the direction of the swipe gesture
    /// </summary>
    public SwipeDirection SwipeDirection { get; }

    /// <summary>
    /// Gets whether the swipe items remain visible after the gesture completes
    /// </summary>
    public bool IsOpen { get; }

    public SwipeEndedEventArgs(SwipeDirection swipeDirection, bool isOpen)
    {
        SwipeDirection = swipeDirection;
        IsOpen = isOpen;
    }

    public SwipeEndedEventArgs(RoutedEvent? routedEvent, SwipeDirection swipeDirection, bool isOpen) : base(routedEvent)
    {
        SwipeDirection = swipeDirection;
        IsOpen = isOpen;
    }
}
