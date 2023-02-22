using System;
using Avalonia.Interactivity;

namespace Avalonia.Labs.Controls
{
    public partial class AsyncImage
    {
        public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
            RoutedEvent.Register<AsyncImage, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);

        public static readonly RoutedEvent<RoutedEventArgs> FailedEvent =
            RoutedEvent.Register<AsyncImage, RoutedEventArgs>(nameof(Failed), RoutingStrategies.Bubble);


        public event EventHandler<RoutedEventArgs>? Opened
        {
            add => AddHandler(OpenedEvent, value);
            remove => RemoveHandler(OpenedEvent, value);
        }

        public event EventHandler<RoutedEventArgs>? Failed
        {
            add => AddHandler(FailedEvent, value);
            remove => RemoveHandler(FailedEvent, value);
        }
    }
}
