using SkiaSharp;
using System;

namespace Avalonia.Labs.Catalog;

public enum BitmapStretch
{
    None,
    Fill,
    Uniform,
    UniformToFill,
    AspectFit = Uniform,
    AspectFill = UniformToFill
}

public enum BitmapAlignment
{
    Start,
    Center,
    End
}

static class SkiaExtensions
{
    public static SKBitmap Rotate(this SKBitmap bitmap, double angle)
    {
        double radians = Math.PI * angle / 180;
        float sine = (float)Math.Abs(Math.Sin(radians));
        float cosine = (float)Math.Abs(Math.Cos(radians));
        int originalWidth = bitmap.Width;
        int originalHeight = bitmap.Height;
        int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
        int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

        var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

        using (var surface = new SKCanvas(rotatedBitmap))
        {
            surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
            surface.RotateDegrees((float)angle);
            surface.Translate(-originalWidth / 2, -originalHeight / 2);
            surface.DrawBitmap(bitmap, 0, 0);
        }
        return rotatedBitmap;
    }

    public static void DrawBitmap(this SKCanvas canvas, SKBitmap bitmap, SKRect dest,
                          BitmapStretch stretch,
                          BitmapAlignment horizontal = BitmapAlignment.Center,
                          BitmapAlignment vertical = BitmapAlignment.Center,
                          SKPaint? paint = default)
    {
        if (stretch == BitmapStretch.Fill)
        {
            canvas.DrawBitmap(bitmap, dest, paint);
        }
        else
        {
            float scale = 1;

            switch (stretch)
            {
                case BitmapStretch.None:
                    break;

                case BitmapStretch.Uniform:
                    scale = Math.Min(dest.Width / bitmap.Width, dest.Height / bitmap.Height);
                    break;

                case BitmapStretch.UniformToFill:
                    scale = Math.Max(dest.Width / bitmap.Width, dest.Height / bitmap.Height);
                    break;
            }

            SKRect display = CalculateDisplayRect(dest, scale * bitmap.Width, scale * bitmap.Height,
                                                  horizontal, vertical);

            canvas.DrawBitmap(bitmap, display, paint);
        }
    }

    static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight,
                               BitmapAlignment horizontal, BitmapAlignment vertical)
    {
        float x = 0;
        float y = 0;

        switch (horizontal)
        {
            case BitmapAlignment.Center:
                x = (dest.Width - bmpWidth) / 2;
                break;

            case BitmapAlignment.Start:
                break;

            case BitmapAlignment.End:
                x = dest.Width - bmpWidth;
                break;
        }

        switch (vertical)
        {
            case BitmapAlignment.Center:
                y = (dest.Height - bmpHeight) / 2;
                break;

            case BitmapAlignment.Start:
                break;

            case BitmapAlignment.End:
                y = dest.Height - bmpHeight;
                break;
        }

        x += dest.Left;
        y += dest.Top;

        return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
    }

    public static SKBitmap Clone(this SKBitmap source)
    {
        var cropRect = SKRect.Create(source.Width, source.Height);
        var result = new SKBitmap((int)cropRect.Width,
                          (int)cropRect.Height);
        using (var canvas = new SKCanvas(result))
        using (var paint = new SKPaint())
        {
            canvas.DrawBitmap(source, cropRect, paint);
        }
        return result;
    }

    public static unsafe SKPixmap ApplyThreshold(this SKBitmap image, byte threshold)
    {
        SKPixmap pixmap = image.PeekPixels();
        if (image.ColorType != SKColorType.Rgba8888)
        {
            using (var old = pixmap)
                pixmap = old.WithColorType(SKColorType.Rgba8888);
        }

        byte* bmpPtr = (byte*)pixmap.GetPixels().ToPointer();
        int width = image.Width;
        int height = image.Height;
        byte* tempPtr;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                tempPtr = bmpPtr;
                byte red = *bmpPtr++;
                byte green = *bmpPtr++;
                byte blue = *bmpPtr++;
                byte alpha = *bmpPtr++;

                // Assuming SKColorType.Rgba8888 - used by iOS and Android
                // (UWP uses SKColorType.Bgra8888)
                byte result = (byte)(0.2126 * red + 0.7152 * green + 0.0722 * blue);
                if (result > threshold)
                {
                    result = 255;
                }
                else
                {
                    result = 0;
                }

                bmpPtr = tempPtr;
                *bmpPtr++ = result; // red
                *bmpPtr++ = result; // green
                *bmpPtr++ = result; // blue
                *bmpPtr++ = alpha;  // alpha
            }
        }
        return pixmap;
    }
}
