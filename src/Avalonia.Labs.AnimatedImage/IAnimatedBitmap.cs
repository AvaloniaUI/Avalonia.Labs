using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.AnimatedImage;

public interface IAnimatedBitmap
{
    bool IsInitialized { get; }

    bool IsFailed { get; }

    bool IsCancellable { get; set; }
    
    /// <summary>
    /// Gets the size of the image, in device independent pixels.
    /// </summary>
    Size Size { get; }
    
    int FrameCount { get; }

    IReadOnlyList<Bitmap> Frames { get; }

    IReadOnlyList<int> Delays { get; }

    void Init();
    
    event EventHandler? Initialized;
    
    event EventHandler<AnimatedBitmapFailedEventArgs>? Failed;

    static IAnimatedBitmap Load(Stream stream, bool disposeStream)
        => new SingleAnimatedBitmap(stream, disposeStream);

    static IAnimatedBitmap Load(IReadOnlyCollection<Stream> frameStreams, IReadOnlyCollection<int> delays, bool disposeStream)
        => new MultiAnimatedBitmap(frameStreams, delays, disposeStream);

    static IAnimatedBitmap Load(IReadOnlyCollection<Bitmap> bitmaps, IReadOnlyCollection<int> delays)
        => new AnimatedBitmapSimpleImpl(bitmaps, delays);
}
