using Avalonia.Labs.Controls;
using SkiaSharp;

namespace Avalonia.Labs.Catalog.Controls;

internal abstract class BindableCanvas : SKCanvasView
{
    public static readonly AvaloniaProperty<SKBitmap?> SourceProperty =
        AvaloniaProperty.Register<BindableCanvas, SKBitmap?>(nameof(Source));

    public SKBitmap? Source
    {
        get => this.GetValue<SKBitmap?>(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public abstract SKBitmap? Result { get; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty)
        {
            InvalidateSurface();
        }
    }

    sealed protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var bitmap = Source;
        if (IsVisible == false || bitmap == null)
        {
            return;
        }
        var info = e.Info;
        var surface = e.Surface;
        var canvas = surface.Canvas;
        OnPaintSurface(info, canvas, bitmap);
        base.OnPaintSurface(e);
    }

    protected virtual void OnPaintSurface(SKImageInfo info, SKCanvas canvas, SKBitmap source)
    {
        canvas.Clear();
        if (source != null)
        {
            canvas.DrawBitmap(source, info.Rect, BitmapStretch.Uniform);
        }
    }
}
