using System;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Skia;
using SkiaSharp;

namespace Avalonia.Labs.Lottie;

internal class LottieCompositionCustomVisualHandler : CompositionCustomVisualHandler
{
    private TimeSpan _primaryTimeElapsed, _animationElapsed;
    private TimeSpan? _lastServerTime;
    private bool _running;
    private bool _paused;
    private SkiaSharp.Skottie.Animation? _animation;
    private Stretch? _stretch;
    private StretchDirection? _stretchDirection;
    private SkiaSharp.SceneGraph.InvalidationController? _ic;
    private readonly object _sync = new();
    private int _repeatCount;
    private int _count;
    private int _playBackRate;
    private Action? _onAnimationCompleted;
    private Action<int>? _onAnimationCompletedRepetition;

    public override void OnMessage(object message)
    {
        if (message is not LottiePayload msg)
        {
            return;
        }

        switch (msg)
        {
            case
            {
                LottieCommand: LottieCommand.Start,
                Animation: { } an,
                Stretch: { } st,
                StretchDirection: { } sd,
                RepeatCount: { } rp,
                PlayBackRate: { } pbr
            }:
            {
                _running = true;
                _paused = false;
                _lastServerTime = null;
                _stretch = st;
                _stretchDirection = sd;
                _animation = an;
                _repeatCount = rp;
                _playBackRate = pbr;
                _count = 0;
                _animationElapsed = TimeSpan.Zero;
                _onAnimationCompleted = msg.OnAnimationCompleted;
                _onAnimationCompletedRepetition = msg.OnAnimationCompletedRepetition;
                RegisterForNextAnimationFrameUpdate();
                break;
            }
            case
            {
                LottieCommand: LottieCommand.Pause
            }:
            {
                _paused = true;
                break;
            }
            case
            {
                LottieCommand: LottieCommand.Resume
            }:
            {
                _paused = false;
                RegisterForNextAnimationFrameUpdate();
                break;
            }
            case
            {
                LottieCommand: LottieCommand.Seek
            }:
            {
                if (msg.SeekFrame.HasValue)
                {
                    SeekToFrame(msg.SeekFrame.Value);
                }
                else if (msg.SeekProgress.HasValue)
                {
                    SeekToProgress(msg.SeekProgress.Value);
                }
                break;
            }
            case
            {
                LottieCommand: LottieCommand.Update,
                Stretch: { } st,
                StretchDirection: { } sd
            }:
            {
                _stretch = st;
                _stretchDirection = sd;
                RegisterForNextAnimationFrameUpdate();
                break;
            }
            case
            {
                LottieCommand: LottieCommand.Stop
            }:
            {
                _running = false;
                _animationElapsed = TimeSpan.Zero;
                _count = 0;
                break;
            }
            case
            {
                LottieCommand: LottieCommand.Dispose
            }:
            {
                DisposeImpl();
                break;
            }
        }
    }

    public override void OnAnimationFrameUpdate()
    {
        if (!_running || _paused)
            return;

        if (_lastServerTime.HasValue)
        {
            var delta = CompositionNow - _lastServerTime.Value;
            _primaryTimeElapsed += delta;
            _animationElapsed += delta;
        }

        _lastServerTime = CompositionNow;

        GetFrameTime(); // This will handle cycle completion and repetitions

        if (!_running)
            return; // Animation has completed all repetitions

        Invalidate();
        RegisterForNextAnimationFrameUpdate();
    }

    private void SeekToFrame(float frame)
    {
        if (_animation == null)
        {
            return;
        }

        _animationElapsed = TimeSpan.FromSeconds(frame / _animation.Fps);
        Invalidate();
        RegisterForNextAnimationFrameUpdate();
    }

    private void SeekToProgress(float progress)
    {
        if (_animation == null)
        {
            return;
        }

        progress = Math.Min(Math.Max(progress, 0), 1);
        _animationElapsed = TimeSpan.FromSeconds(_animation.Duration.TotalSeconds * progress);
        Invalidate();
        RegisterForNextAnimationFrameUpdate();
    }

    private void DisposeImpl()
    {
        lock (_sync)
        {
            _animation?.Dispose();
            _animation = null;
            _ic?.End();
            _ic?.Dispose();
            _ic = null;
        }
    }

    private double GetFrameTime()
    {
        if (_animation is null)
        {
            return 0f;
        }

        var frameTime = _animationElapsed.TotalSeconds * _playBackRate;

        if (frameTime > _animation.Duration.TotalSeconds)
        {
            _animationElapsed = TimeSpan.Zero;
            _ic?.End();
            _ic?.Begin();
            _count++;

            if (_repeatCount != Lottie.Infinity && _count >= _repeatCount)
            {
                // Animation has finished all repetitions
                _running = false;
                _onAnimationCompleted?.Invoke();
                return _animation.Duration.TotalSeconds; // Return the last frame
            }

            // Animation cycle completed, but not finished all repetitions
            _onAnimationCompletedRepetition?.Invoke(_count);

            frameTime = 0; // Reset frame time for the next cycle
        }

        return frameTime;
    }

    private void Draw(SKCanvas canvas)
    {
        var animation = _animation;
        if (animation is null)
        {
            return;
        }

        if (_ic is null)
        {
            _ic = new SkiaSharp.SceneGraph.InvalidationController();
            _ic.Begin();
        }

        var ic = _ic;

        if (_repeatCount == 0)
        {
            return;
        }

        var t = GetFrameTime();
        if (!_running)
        {
            t = (float)animation.Duration.TotalSeconds;
        }

        var dst = new SKRect(0, 0, animation.Size.Width, animation.Size.Height);

        animation.SeekFrameTime(t, ic);
        canvas.Save();
        animation.Render(canvas, dst);
        canvas.Restore();

        ic.Reset();
    }

    public override void OnRender(ImmediateDrawingContext context)
    {
        lock (_sync)
        {
            if (_running)
            {
                if (_lastServerTime.HasValue)
                {
                    var delta = (CompositionNow - _lastServerTime.Value);
                    _primaryTimeElapsed += delta;
                    _animationElapsed += delta;
                }

                _lastServerTime = CompositionNow;
            }

            if (_animation is not { } an 
                || _stretch is not { } st 
                || _stretchDirection is not { } sd)
            {
                return;
            }


            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null)
            {
                return;
            }

            var rb = GetRenderBounds();

            var viewPort = new Rect(rb.Size);
            var sourceSize = new Size(an.Size.Width, an.Size.Height);
            if (sourceSize.Width <= 0 || sourceSize.Height <= 0)
            {
                return;
            }

            var scale = st.CalculateScaling(rb.Size, sourceSize, sd);
            var scaledSize = sourceSize * scale;
            var destRect = viewPort
                .CenterRect(new Rect(scaledSize))
                .Intersect(viewPort);
            var sourceRect = new Rect(sourceSize)
                .CenterRect(new Rect(destRect.Size / scale));

            var bounds = SKRect.Create(new SKPoint(), an.Size);
            var scaleMatrix = Matrix.CreateScale(
                destRect.Width / sourceRect.Width,
                destRect.Height / sourceRect.Height);
            var translateMatrix = Matrix.CreateTranslation(
                -sourceRect.X + destRect.X - bounds.Top,
                -sourceRect.Y + destRect.Y - bounds.Left);

            using (context.PushClip(destRect))
            using (context.PushPostTransform(translateMatrix * scaleMatrix))
            {
                using var lease = leaseFeature.Lease();
                var canvas = lease?.SkCanvas;
                if (canvas is null)
                {
                    return;
                }
                Draw(canvas);
            }
        }
    }
}
