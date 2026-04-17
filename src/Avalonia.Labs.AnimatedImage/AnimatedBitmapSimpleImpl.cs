using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.AnimatedImage;

internal class AnimatedBitmapSimpleImpl : IAnimatedBitmap
{
    public AnimatedBitmapSimpleImpl(IReadOnlyCollection<Bitmap> bitmaps, IReadOnlyCollection<int> delays)
    {
        ArgumentNullException.ThrowIfNull(bitmaps);
        ArgumentNullException.ThrowIfNull(delays);
        if (bitmaps.Count is var bitmapCount && delays.Count != bitmapCount)
            throw new ArgumentException($"{nameof(delays)} inconsistent count with {nameof(bitmaps)}");
        if ((IReadOnlyList<Bitmap>) [.. bitmaps] is not [var first, ..] bitmapsCopy)
            throw new ArgumentException($"Invalid {nameof(bitmaps)}.Count");
        Size = first.Size;
        Frames = bitmapsCopy;
        Delays = [.. delays];
        FrameCount = bitmapCount;
    }

    public bool IsInitialized { get; set; } = true;

    public bool IsFailed => false;

    public bool IsCancellable { get; set; }

    public bool IsDisposed { get; set; }

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
