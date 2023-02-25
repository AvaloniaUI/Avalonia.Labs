using System;
using Avalonia.Interactivity;

namespace Avalonia.Labs.Controls
{
    public partial class AsyncImage
    {
        /// <summary>
        /// Deines the <see cref="Opened"/> event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
            RoutedEvent.Register<AsyncImage, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);

        /// <summary>
        /// Deines the <see cref="Failed"/> event
        /// </summary>
        public static readonly RoutedEvent<AsyncImageFailedEventArgs> FailedEvent =
            RoutedEvent.Register<AsyncImage, AsyncImageFailedEventArgs>(nameof(Failed), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when the image is successfully loaded.
        /// </summary>
        public event EventHandler<RoutedEventArgs>? Opened
        {
            add => AddHandler(OpenedEvent, value);
            remove => RemoveHandler(OpenedEvent, value);
        }

        /// <summary>
        /// Occurs when the image fails to load the uri provided.
        /// </summary>
        public event EventHandler<AsyncImageFailedEventArgs>? Failed
        {
            add => AddHandler(FailedEvent, value);
            remove => RemoveHandler(FailedEvent, value);
        }
    }
}
