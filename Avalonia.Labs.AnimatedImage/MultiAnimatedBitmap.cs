using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.AnimatedImage;

internal class MultiAnimatedBitmap(IReadOnlyCollection<Stream> frameStreams, IReadOnlyCollection<int> delays, bool disposeStream) : IAnimatedBitmap
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

    private List<Stream>? _frameStreams =
        (frameStreams ?? throw new ArgumentNullException(nameof(frameStreams))).Count < 1
            ? throw new ArgumentException($"Invalid {nameof(frameStreams)}.Count")
            : [..frameStreams];

    private readonly IReadOnlyCollection<int> _delays =
        (IReadOnlyCollection<int>) [..delays] ?? throw new ArgumentNullException(nameof(delays));

    public void Init()
    {
        if (IsInitialized || IsFailed)
            return;
        try
        {
            if (_frameStreams is null)
                throw new NullReferenceException(nameof(_frameStreams));
            var delays = new int[_frameStreams.Count];
            var frames = new Bitmap[_frameStreams.Count];
            var index = 0;
            while (_frameStreams.Count > 0)
            {
                delays[index] = _delays.ElementAtOrDefault(index) is var delay && delay > 0 ? delay : 100;
                var frameStream = _frameStreams[0];
                try
                {
                    frames[index] = new Bitmap(frameStream);
                }
                finally
                {
                    if (disposeStream)
                        frameStream.Dispose();
                }

                _frameStreams.RemoveAt(0);

                ++index;
            }
            _frameStreams = null;

            Size = frames[0].Size;
            FrameCount = delays.Length;
            Delays = delays;
            Frames = frames;
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            if (_frameStreams is not null && disposeStream)
                foreach (var frameStream in _frameStreams)
                    frameStream.Dispose();
            _frameStreams = null;
            IsFailed = true;
            Failed?.Invoke(this, new AnimatedBitmapFailedEventArgs(e));
        }
    }
}
