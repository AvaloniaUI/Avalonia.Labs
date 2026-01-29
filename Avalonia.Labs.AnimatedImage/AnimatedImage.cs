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

    public static readonly StyledProperty<IAnimatedBitmap?> SourceProperty = AvaloniaProperty.Register<AnimatedImage, IAnimatedBitmap?>(name: nameof(Source), defaultValue: null);

    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty = AvaloniaProperty.Register<AnimatedImage, StretchDirection>(nameof(StretchDirection), StretchDirection.Both);

    public static readonly StyledProperty<Stretch> StretchProperty = AvaloniaProperty.Register<AnimatedImage, Stretch>(nameof(Stretch), Stretch.UniformToFill);

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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        switch (change.Property.Name)
        {
            case nameof(Source):
                OnSourcePropertyChanged(change.NewValue as IAnimatedBitmap);
                break;
            case nameof(Stretch):
                InvalidateArrange();
                InvalidateMeasure();
                _customVisual?.SendHandlerMessage(Stretch);
                Update();
                break;
            case nameof(StretchDirection):
                InvalidateArrange();
                InvalidateMeasure();
                _customVisual?.SendHandlerMessage(StretchDirection);
                Update();
                break;
            case nameof(Bounds):
                Update();
                break;
        }

        base.OnPropertyChanged(change);
    }
    
    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var compositor = ElementComposition.GetElementVisual(this)?.Compositor;
        if (compositor is null || _customVisual?.Compositor == compositor)
            return;
        _customVisual = compositor.CreateCustomVisual(new CustomVisualHandler());
        ElementComposition.SetElementChildVisual(this, _customVisual);
        _customVisual.SendHandlerMessage(CustomVisualHandler.StartMessage);
        
        if (Source is { IsInitialized: false, IsFailed: false } source)
            await InitSourceAsync(source);
        _customVisual.SendHandlerMessage(Stretch);
        _customVisual.SendHandlerMessage(StretchDirection);
        if (Source is { IsInitialized: true })
            _customVisual.SendHandlerMessage(Source);
        Update();
        base.OnAttachedToVisualTree(e);
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
        if (_customVisual is null)
            return;

        if (newValue is null)
            _customVisual.SendHandlerMessage(CustomVisualHandler.ResetMessage);
        else
        {
            if (Source is { IsInitialized: false, IsFailed: false } source)
                await InitSourceAsync(source);
            if (Source is { IsInitialized: true })
                _customVisual.SendHandlerMessage(newValue);
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

    private async Task InitSourceAsync(IAnimatedBitmap source)
    {
        if (source.IsCancellable)
        {
            await Task.Run(async () => await source.InitAsync());
            return;
        }

        if (_cancellationTokenSource is not null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        _cancellationTokenSource = new();
        try
        {
            await Task.Run(async () => await source.InitAsync(_cancellationTokenSource.Token));
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

            var ms = (int) _animationElapsed.TotalMilliseconds % _totalTime;
            var i = _frameTimes.BinarySearch(ms);
            var bitmap = _currentInstance.Frames[i < 0 ? ~i - 1 : i];

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
