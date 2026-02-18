using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Labs.AnimatedImage;

internal class SingleAnimatedBitmap(Stream stream, bool disposeStream) : IAnimatedBitmap
{
    public bool IsInitialized { get => !IsFailed && field; private set; }

    public bool IsFailed { get; private set; }

    public bool IsCancellable { get; set; }

    public Size Size { get; private set; }
    
    public int FrameCount { get; private set; }

    [field: MaybeNull, AllowNull]
    public IReadOnlyList<Bitmap> Frames
    {
        get => field ?? throw new InvalidOperationException();
        private set;
    }

    public IReadOnlyList<int> Delays { get; private set; } = [];

    public event EventHandler? Initialized;
    
    public event EventHandler<AnimatedBitmapFailedEventArgs>? Failed;
    
    private Stream? _stream = stream ?? throw new ArgumentNullException(nameof(stream));

    public void Init()
    {
        if (IsInitialized || IsFailed)
            return;
        try
        {
            if (_stream is null)
                throw new NullReferenceException(nameof(_stream));

            if (_stream.CanSeek)
                _stream.Position = 0;

            using var skCodec = SKCodec.Create(_stream);
            if (skCodec is null)
                throw new InvalidOperationException($"Unable to create {nameof(SKCodec)} from the provided stream.");

            var imageInfo = skCodec.Info;
            var targetInfo = new SKImageInfo(imageInfo.Width, imageInfo.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            var frameCount = Math.Max(skCodec.FrameCount, 1);
            var delays = new int[frameCount];
            var frames = new Bitmap[frameCount];
            var frameInfos = skCodec.FrameInfo;

            for (var index = 0; index < frameCount; index++)
            {
                var frameInfo = frameInfos.Length > index ? frameInfos[index] : default;
                delays[index] = frameInfo.Duration > 0 ? frameInfo.Duration : 100;
                frames[index] = DecodeFrame(skCodec, targetInfo, index);
            }

            if (disposeStream)
                _stream.Dispose();
            _stream = null;

            Size = new Size(imageInfo.Width, imageInfo.Height);
            FrameCount = delays.Length;
            Delays = delays;
            Frames = frames;
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            if (_stream is not null && disposeStream)
                _stream.Dispose();
            _stream = null;
            IsFailed = true;
            Failed?.Invoke(this, new AnimatedBitmapFailedEventArgs(e));
        }
    }

    private static Bitmap DecodeFrame(SKCodec codec, SKImageInfo imageInfo, int frameIndex)
    {
        using var bitmap = new SKBitmap(imageInfo);
        var options = new SKCodecOptions(frameIndex);
        var result = codec.GetPixels(imageInfo, bitmap.GetPixels(), options);
        if (result is not SKCodecResult.Success and not SKCodecResult.IncompleteInput)
            throw new InvalidOperationException($"Failed to decode frame {frameIndex}: {result}.");

        return new Bitmap(
            PixelFormat.Bgra8888,
            AlphaFormat.Premul,
            bitmap.GetPixels(),
            new PixelSize(imageInfo.Width, imageInfo.Height),
            new Vector(96, 96),
            bitmap.RowBytes);
    }
}
