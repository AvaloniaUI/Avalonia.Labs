using System;

namespace Avalonia.Labs.AnimatedImage;

public class AnimatedBitmapFailedEventArgs(Exception exception) : EventArgs
{
    public Exception Exception { get; set; } = exception;
}
