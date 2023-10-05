using SkiaSharp;
using System;

namespace Avalonia.Labs.Controls;

public sealed class SKPaintSurfaceEventArgs : EventArgs
{
    public SKPaintSurfaceEventArgs(SKSurface surface, SKImageInfo info)
        : this(surface, info, info)
    {
    }

    public SKPaintSurfaceEventArgs(SKSurface surface, SKImageInfo info, SKImageInfo rawInfo)
    {
        Surface = surface;
        Info = info;
        RawInfo = rawInfo;
    }

    public SKSurface Surface { get; }

    public SKImageInfo Info { get; }

    public SKImageInfo RawInfo { get; }
}
