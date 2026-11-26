using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Animation;
using Avalonia.Labs.Gif.Decoding;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Avalonia.Labs.Gif;

internal class GifInstance : IDisposable
{
    public IterationCount IterationCount { get; set; }
    private readonly GifDecoder _gifDecoder;
    private readonly WriteableBitmap? _targetBitmap;
    private TimeSpan _totalTime;
    private readonly List<TimeSpan>? _frameTimes;
    private int _currentFrameIndex;

    private CancellationTokenSource CurrentCts { get; }

    internal GifInstance(object newValue) : this(newValue switch
    {
        Stream s => s,
        _ => throw new InvalidDataException("Unsupported source object")
    })
    {
    }

    private GifInstance(Stream currentStream)
    {
        if (!currentStream.CanSeek)
            throw new InvalidDataException("The provided stream is not seekable.");

        if (!currentStream.CanRead)
            throw new InvalidOperationException("Can't read the stream provided.");

        currentStream.Seek(0, SeekOrigin.Begin);

        CurrentCts = new CancellationTokenSource();

        _gifDecoder = new GifDecoder(currentStream, CurrentCts.Token);

        if (_gifDecoder.Header is null)
            return;

        var pixSize = new PixelSize(_gifDecoder.Header.Dimensions.Width, _gifDecoder.Header.Dimensions.Height);

        _targetBitmap = new WriteableBitmap(pixSize, new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        GifPixelSize = pixSize;

        _totalTime = TimeSpan.Zero;

        _frameTimes = _gifDecoder._frames.Select(frame =>
        {
            _totalTime = _totalTime.Add(frame.FrameDelay);
            return _totalTime;
        }).ToList();

        _gifDecoder.RenderFrame(0, _targetBitmap);
    }

    public PixelSize GifPixelSize { get; }
    private bool _isDisposed;

    public void Dispose()
    {
        if (_isDisposed)
            return;

        GC.SuppressFinalize(this);

        _isDisposed = true;
        CurrentCts.Cancel();
        _targetBitmap?.Dispose();
    }

    public WriteableBitmap? ProcessFrameTime(TimeSpan elapsed)
    {
        if (CurrentCts.IsCancellationRequested)
        {
            return null;
        }

        if (_frameTimes is null)
            return null;

        var totalTicks = _totalTime.Ticks;

        if (totalTicks == 0)
        {
            return ProcessFrameIndex(0);
        }

        var elapsedTicks = elapsed.Ticks;

        // Once all iterations have played, hold the frame the animation ended on.
        if (!IterationCount.IsInfinite && (ulong)(elapsedTicks / totalTicks) >= IterationCount.Value)
        {
            return ProcessFrameIndex(IterationCount.Value == 0 ? 0 : _frameTimes.Count - 1);
        }

        var timeModulus = TimeSpan.FromTicks(elapsedTicks % totalTicks);
        var targetFrame = _frameTimes.FirstOrDefault(x => timeModulus < x);
        var currentFrame = _frameTimes.IndexOf(targetFrame);
        if (currentFrame == -1)
            currentFrame = 0;

        if (_currentFrameIndex == currentFrame)
            return _targetBitmap;

        return ProcessFrameIndex(currentFrame);
    }

    private WriteableBitmap? ProcessFrameIndex(int frameIndex)
    {
        _gifDecoder.RenderFrame(frameIndex, _targetBitmap);
        _currentFrameIndex = frameIndex;

        return _targetBitmap;
    }
}
