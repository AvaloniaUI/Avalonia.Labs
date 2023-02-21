using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public class AsyncImageFailedEventArgs : RoutedEventArgs
        {
            internal AsyncImageFailedEventArgs(Exception? errorException = null, string errorMessage = "") : base(FailedEvent)
            {
                ErrorException = errorException;
                ErrorMessage = errorMessage;
            }

            public Exception? ErrorException { get; private set; }
            public string ErrorMessage { get; private set; }
        }
    }
}
