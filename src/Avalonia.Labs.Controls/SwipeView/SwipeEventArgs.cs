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
