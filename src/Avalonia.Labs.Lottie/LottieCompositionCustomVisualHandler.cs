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
    private SkiaSharp.Skottie.Animation? _animation;
    private Stretch? _stretch;
    private StretchDirection? _stretchDirection;
    private SkiaSharp.SceneGraph.InvalidationController? _ic;
    private readonly object _sync = new();
    private int _repeatCount;
    private int _count;

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
                RepeatCount: { } rp
            }:
            {
                _running = true;
                _lastServerTime = null;
                _stretch = st;
                _stretchDirection = sd;
                _animation = an;
                _repeatCount = rp;
                _count = 0;
                _animationElapsed = TimeSpan.Zero;
                RegisterForNextAnimationFrameUpdate();
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
        if (!_running)
            return;


        if (_repeatCount == 0 || (_repeatCount > 0 && _count >= _repeatCount))
        {
            _running = false;
            _animationElapsed = TimeSpan.Zero;
        }

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

        var frameTime = _animationElapsed.TotalSeconds;

        if (_animationElapsed.TotalSeconds > _animation.Duration.TotalSeconds)
        {
            _animationElapsed = TimeSpan.Zero;
            _ic?.End();
            _ic?.Begin();
            _count++;
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
