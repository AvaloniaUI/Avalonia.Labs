using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.AnimatedImage;

internal class AnimatedBitmapSimpleImpl : IAnimatedBitmap
{
    public AnimatedBitmapSimpleImpl(IReadOnlyCollection<Bitmap> bitmaps, IReadOnlyCollection<int> delays)
    {
        if (bitmaps is null)
            throw new ArgumentNullException(nameof(bitmaps));
        if (delays is null)
            throw new ArgumentNullException(nameof(delays));
        if (bitmaps.Count is var bitmapCount && delays.Count != bitmapCount)
            throw new ArgumentException($"{nameof(delays)} inconsistent count with {nameof(bitmaps)}");
        if ((IReadOnlyList<Bitmap>) [..bitmaps] is not [var first, ..] bitmapsCopy)
            throw new ArgumentException($"Invalid {nameof(delays)}.Count");
        Size = first.Size;
        Frames = bitmapsCopy;
        Delays = [..delays];
        FrameCount = bitmapCount;
    }

    public bool IsInitialized => true;

    public bool IsFailed => false;

    public bool IsCancellable { get; set; }

    public Size Size { get; } 

    public int FrameCount { get; }

    [field: MaybeNull, AllowNull]
    public IReadOnlyList<Bitmap> Frames { get; }

    public IReadOnlyList<int> Delays { get; }

    public event EventHandler? Initialized;
    
    public event EventHandler<AnimatedBitmapFailedEventArgs>? Failed;
    
    public void Init()
    {
    }
}
