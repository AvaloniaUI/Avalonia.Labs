using System;
using Avalonia.Interactivity;

namespace Avalonia.Labs.Controls
{
    public partial class AsyncImage
    {
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
