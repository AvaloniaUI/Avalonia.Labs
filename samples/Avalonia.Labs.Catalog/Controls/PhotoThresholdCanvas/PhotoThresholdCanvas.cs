using System;
using SkiaSharp;

namespace Avalonia.Labs.Catalog.Controls;

internal class PhotoThresholdCanvas : BindableCanvas
{
    public PhotoThresholdCanvas()
    {
    }

    public readonly static AvaloniaProperty<byte> ThresholdProperty =
        AvaloniaProperty.Register<PhotoThresholdCanvas, byte>(nameof(Threshold), 128, defaultBindingMode: Data.BindingMode.TwoWay);

    public byte Threshold
    {
        get => this.GetValue<byte>(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    public override SKBitmap? Result
    {
        get
        {
            SKBitmap? result = default;

            if (Source is SKBitmap bitmap)
            {
                var cropRect = SKRect.Create(bitmap.Width, bitmap.Height);
                result = new SKBitmap((int)cropRect.Width,
                                  (int)cropRect.Height);
                using (var canvas = new SKCanvas(result))
                {
                    OnPaintSurface(canvas, bitmap, cropRect, Threshold);
                }
            }

            return result;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ThresholdProperty)
        {
            InvalidateSurface();
        }
        base.OnPropertyChanged(change);
    }

    protected override void OnPaintSurface(SKCanvas canvas, SKBitmap source)
    {
        canvas.Clear();
        var info = CanvasSize;
        float scale = Math.Min((float)info.Width / source.Width, (float)info.Height / source.Height);
        float x = (float)(info.Width - scale * source.Width) / 2;
        float y = (float)(info.Height - scale * source.Height) / 2;
        var bitmapRect = new SKRect(x, y, x + scale * source.Width, y + scale * source.Height);
        var threshold = Threshold;
        OnPaintSurface(canvas, source, bitmapRect, threshold);
    }

    private static void OnPaintSurface(SKCanvas canvas, SKBitmap source, SKRect bitmapRect, byte threshold)
    {
        canvas.DrawImage(SKImage.FromPixels(source.Clone().ApplyThreshold(threshold)), bitmapRect);
    }
}
