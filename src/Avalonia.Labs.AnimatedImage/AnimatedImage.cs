using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Rendering.Composition;

namespace Avalonia.Labs.AnimatedImage;

public class AnimatedImage : Control
{
    private CompositionCustomVisual? _customVisual;
    private CancellationTokenSource? _cancellationTokenSource;
    private int _visualTreeVersion;

    public static readonly StyledProperty<IAnimatedBitmap?> SourceProperty = AvaloniaProperty.Register<AnimatedImage, IAnimatedBitmap?>(nameof(Source), defaultValue: null);

    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty = AvaloniaProperty.Register<AnimatedImage, StretchDirection>(nameof(StretchDirection), StretchDirection.Both);

    public static readonly StyledProperty<Stretch> StretchProperty = AvaloniaProperty.Register<AnimatedImage, Stretch>(nameof(Stretch), Stretch.UniformToFill);

    public static readonly StyledProperty<bool> IsPlayingProperty = AvaloniaProperty.Register<AnimatedImage, bool>(nameof(IsPlaying), true);

    [Content]
    public IAnimatedBitmap? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public bool IsPlaying
    {
        get => GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    static AnimatedImage()
    {
        AffectsMeasure<AnimatedImage>(SourceProperty, StretchProperty, StretchDirectionProperty);
        AffectsArrange<AnimatedImage>(SourceProperty, StretchProperty, StretchDirectionProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        switch (change.Property.Name)
        {
            case nameof(Source):
                OnSourcePropertyChanged(change.NewValue as IAnimatedBitmap);
                break;
            case nameof(IsPlaying):
                _customVisual?.SendHandlerMessage(IsPlaying
                    ? CustomVisualHandler.StartMessage
                    : CustomVisualHandler.StopMessage);
                break;
            case nameof(Stretch):
                _customVisual?.SendHandlerMessage(Stretch);
                Update();
                break;
            case nameof(StretchDirection):
                _customVisual?.SendHandlerMessage(StretchDirection);
                Update();
                break;
            case nameof(Bounds):
                Update();
                break;
        }

        base.OnPropertyChanged(change);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var version = Interlocked.Increment(ref _visualTreeVersion);
        _ = EnsureCustomVisualStateAsync(version);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _ = Interlocked.Increment(ref _visualTreeVersion);

        _customVisual?.SendHandlerMessage(CustomVisualHandler.StopMessage);

        ElementComposition.SetElementChildVisual(this, null);
        _customVisual = null;

        base.OnDetachedFromVisualTree(e);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        return Source is { IsInitialized: true }
            ? Stretch.CalculateSize(availableSize, Source.Size, StretchDirection)
            : default;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        return Source is { IsInitialized: true }
            ? Stretch.CalculateSize(finalSize, Source.Size)
            : default;
    }

    private async void OnSourcePropertyChanged(IAnimatedBitmap? newValue)
    {
        var customVisual = _customVisual;
        if (customVisual is null)
            return;

        if (newValue is null)
            customVisual.SendHandlerMessage(CustomVisualHandler.ResetMessage);
        else
        {
            if (Source is { IsInitialized: false, IsFailed: false } source)
                await InitSourceAsync(source);

            if (VisualRoot is null || !ReferenceEquals(_customVisual, customVisual))
                return;

            if (Source is { IsInitialized: true })
                customVisual.SendHandlerMessage(newValue);
        }

        InvalidateArrange();
        InvalidateMeasure();
        Update();
    }

    private void Update()
    {
        if (_customVisual is null)
            return;

        _customVisual.Size = new Vector2((float) Bounds.Width, (float) Bounds.Height);
        _customVisual.Offset = Vector3.Zero;
    }

    private async Task EnsureCustomVisualStateAsync(int version)
    {
        var compositor = ElementComposition.GetElementVisual(this)?.Compositor;
        if (compositor is null || version != _visualTreeVersion)
            return;

        // 不复用旧实例，避免“旧父级未解绑 + 新父级绑定”冲突
        var customVisual = compositor.CreateCustomVisual(new CustomVisualHandler());
        _customVisual = customVisual;
        ElementComposition.SetElementChildVisual(this, customVisual);
        customVisual.SendHandlerMessage(IsPlaying
            ? CustomVisualHandler.StartMessage
            : CustomVisualHandler.StopMessage);

        if (VisualRoot is null || version != _visualTreeVersion || !ReferenceEquals(_customVisual, customVisual))
            return;

        customVisual.SendHandlerMessage(Stretch);
        customVisual.SendHandlerMessage(StretchDirection);
        if (Source is { IsInitialized: true })
            customVisual.SendHandlerMessage(Source);

        InvalidateArrange();
        InvalidateMeasure();
        Update();
    }

    private async Task InitSourceAsync(IAnimatedBitmap source)
    {
        if (source.IsCancellable)
        {
            await Task.Run(source.Init);
            return;
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
        }

        _cancellationTokenSource = new();
        try
        {
            await Task.Run(source.Init, _cancellationTokenSource.Token);
        }
        catch
        {
            // ignored
        }
    }

    ~AnimatedImage()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    private class CustomVisualHandler : CompositionCustomVisualHandler
    {
        private TimeSpan _animationElapsed;
        private TimeSpan? _lastServerTime;
        private IAnimatedBitmap? _currentInstance;
        private Stretch _stretch = Stretch.None;
        private StretchDirection _stretchDirection = StretchDirection.Both;
        private int _totalTime;
        private readonly List<int> _frameTimes = [];
        private bool _running;

        public static readonly object StopMessage = new();
        public static readonly object StartMessage = new();
        public static readonly object ResetMessage = new();

        public override void OnMessage(object message)
        {
            if (message == StartMessage)
            {
                _running = true;
                _lastServerTime = null;
                RegisterForNextAnimationFrameUpdate();
            }
            else if (message == StopMessage)
                _running = false;
            else if (message == ResetMessage)
                Clear();
            else switch (message)
            {
                case Stretch st:
                    _stretch = st;
                    break;
                case StretchDirection sd:
                    _stretchDirection = sd;
                    break;
                case IAnimatedBitmap { IsInitialized: true } instance:
                    {
                        Clear();
                        if (instance.Delays.Count != instance.FrameCount)
                            throw new ArgumentException(
                                $"{nameof(instance.Delays)} inconsistent count with {nameof(instance.Frames)}");
                        _currentInstance = instance;
                        foreach (var delay in instance.Delays)
                        {
                            _frameTimes.Add(_totalTime);
                            _totalTime += delay;
                        }

                        Invalidate();

                        break;
                    }
            }
            return;

            void Clear()
            {
                _currentInstance = null;
                _totalTime = 0;
                _frameTimes.Clear();
            }
        }

        public override void OnAnimationFrameUpdate()
        {
            if (!_running)
                return;
            Invalidate();
            RegisterForNextAnimationFrameUpdate();
        }

        public override void OnRender(ImmediateDrawingContext drawingContext)
        {
            if (_running)
            {
                if (_lastServerTime.HasValue)
                    _animationElapsed += CompositionNow - _lastServerTime.Value;
                _lastServerTime = CompositionNow;
            }

            if (_currentInstance is not { IsInitialized: true })
                return;

            var i = 0;
            if (_totalTime > 0)
            {
                var ms = (int) _animationElapsed.TotalMilliseconds % _totalTime;
                i = _frameTimes.BinarySearch(ms);
                i = i < 0 ? ~i - 1 : i;
            }

            var bitmap = _currentInstance.Frames[i];

            var viewPort = GetRenderBounds();
            var bounds = viewPort.Size;
            var sourceSize = _currentInstance.Size;
            var sourceRect = new Rect(sourceSize);

            var scale = _stretch.CalculateScaling(bounds, sourceSize, _stretchDirection);
            var scaledRect = sourceRect * scale;
            var destRect = viewPort
                .CenterRect(scaledRect)
                .Intersect(viewPort);
            sourceRect = sourceRect.CenterRect(destRect / scale);

            // ImmediateDrawingContext 每次OnRender都会清空，所以每次都必须画
            drawingContext.DrawBitmap(bitmap, sourceRect, destRect);
        }
    }
}
