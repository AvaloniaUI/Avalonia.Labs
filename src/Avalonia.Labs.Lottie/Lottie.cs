using System;
using System.IO;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using SkiaSharp;

namespace Avalonia.Labs.Lottie;

/// <summary>
/// Lottie animation player control.
/// </summary>
public class Lottie : Control
{
    private SkiaSharp.Skottie.Animation? _animation;
    private int _repeatCount;
    private int _playBackRate;
    private readonly Uri _baseUri;
    private CompositionCustomVisual? _customVisual;

    /// <summary>
    /// Infinite number of repeats.
    /// </summary>
    public const int Infinity = -1;

    /// <summary>
    /// Defines the <see cref="Path"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PathProperty =
        AvaloniaProperty.Register<Lottie, string?>(nameof(Path));

    /// <summary>
    /// Defines the <see cref="Stretch"/> property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<Lottie, Stretch>(nameof(Stretch), Stretch.Uniform);

    /// <summary>Lottie
    /// Defines the <see cref="StretchDirection"/> property.
    /// </summary>
    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<Lottie, StretchDirection>(
            nameof(StretchDirection),
            StretchDirection.Both);

    /// <summary>
    /// Defines the <see cref="RepeatCount"/> property.
    /// </summary>
    public static readonly StyledProperty<int> RepeatCountProperty =
        AvaloniaProperty.Register<Lottie, int>(nameof(RepeatCount), Infinity);

    /// <summary>
    /// Defines the <see cref="PlayBackRate"/> property.
    /// </summary>
    public static readonly StyledProperty<int> PlayBackRateProperty =
        AvaloniaProperty.Register<Lottie, int>(nameof(PlayBackRate), 1);

    /// <summary>
    /// Defines the <see cref="AutoPlay"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoPlayProperty =
        AvaloniaProperty.Register<Lottie, bool>(nameof(AutoPlay), true);

    /// <summary>
    /// Gets or sets the Lottie animation path.
    /// </summary>
    [Content]
    public string? Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    /// <summary>
    /// Gets or sets a value controlling how the image will be stretched.
    /// </summary>
    public Stretch Stretch
    {
        get { return GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value controlling in what direction the image will be stretched.
    /// </summary>
    public StretchDirection StretchDirection
    {
        get { return GetValue(StretchDirectionProperty); }
        set { SetValue(StretchDirectionProperty, value); }
    }

    /// <summary>
    ///  Sets how many times the animation should be repeated.
    /// </summary>
    public int RepeatCount
    {
        get => GetValue(RepeatCountProperty);
        set => SetValue(RepeatCountProperty, value);
    }

    /// <summary>
    ///  Rate of the animation.
    /// </summary>
    public int PlayBackRate
    {
        get => GetValue(PlayBackRateProperty);
        set => SetValue(PlayBackRateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the animation should automatically play when loaded.
    /// </summary>
    public bool AutoPlay
    {
        get => GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    /// <summary>
    /// Event triggered when the animation completes.
    /// </summary>
    public event EventHandler? AnimationCompleted;

    /// <summary>
    /// Event triggered when the animation completes a repetition. The event argument is the repetition number.
    /// </summary>
    public event EventHandler<int>? AnimationCompletedRepetition;

    /// <summary>
    /// Initializes a new instance of the <see cref="Lottie"/> class.
    /// </summary>
    /// <param name="baseUri">The base URL for the XAML context.</param>
    public Lottie(Uri baseUri)
    {
        _baseUri = baseUri;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lottie"/> class.
    /// </summary>
    /// <param name="serviceProvider">The XAML service provider.</param>
    public Lottie(IServiceProvider serviceProvider)
    {
        _baseUri = serviceProvider.GetContextBaseUri();
    }

    /// <inheritdoc/>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        var elemVisual = ElementComposition.GetElementVisual(this);
        var compositor = elemVisual?.Compositor;
        if (compositor is null)
        {
            return;
        }
        
        _customVisual = compositor.CreateCustomVisual(new LottieCompositionCustomVisualHandler());
        ElementComposition.SetElementChildVisual(this, _customVisual);

        LayoutUpdated += OnLayoutUpdated;

        string? path = Path;
        if (path is null)
        {
            return;
        }

        DisposeImpl();
        Load(path);

        _customVisual.Size = new Vector2((float)Bounds.Size.Width, (float)Bounds.Size.Height);
        _customVisual.SendHandlerMessage(
            new LottiePayload(
                LottieCommand.Update,
                _animation, 
                Stretch, 
                StretchDirection));

        if (AutoPlay)
        {
            Start();
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        LayoutUpdated -= OnLayoutUpdated;

        Stop();
        DisposeImpl();
        _animation = null;
        _customVisual = null;
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (_customVisual == null)
        {
            return;
        }

        _customVisual.Size = new Vector2((float)Bounds.Size.Width, (float)Bounds.Size.Height);
        _customVisual.SendHandlerMessage(
            new LottiePayload(
                LottieCommand.Update, 
                _animation, 
                Stretch, 
                StretchDirection));
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (_animation == null)
        {
            return new Size();
        }

        var sourceSize = _animation is { }
            ? new Size(_animation.Size.Width, _animation.Size.Height)
            : default;

        return Stretch.CalculateSize(availableSize, sourceSize, StretchDirection);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_animation == null)
        {
            return new Size();
        }

        var sourceSize = _animation is { }
            ? new Size(_animation.Size.Width, _animation.Size.Height)
            : default;

        return Stretch.CalculateSize(finalSize, sourceSize);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        switch (change.Property.Name)
        {
            case nameof(Path):
            {
                if (_customVisual is null)
                {
                    return;
                }

                var path = change.GetNewValue<string?>();
                Load(path);
                break;
            }
            case nameof(RepeatCount):
            {
                _repeatCount = change.GetNewValue<int>();
                Stop();

                if (AutoPlay)
                {
                    Start();
                }
                break;
            }
            case nameof(PlayBackRate):
            {
                _playBackRate = change.GetNewValue<int>();
                Stop();
                Start();
                break;
            }
        }
    }

    private SkiaSharp.Skottie.Animation? Load(Stream stream)
    {
        using var managedStream = new SKManagedStream(stream);
        if (SkiaSharp.Skottie.Animation.TryCreate(managedStream, out var animation))
        {
            animation.Seek(0);

            Logger
                .TryGet(LogEventLevel.Information, LogArea.Control)?
                .Log(this, $"Version: {animation.Version} Duration: {animation.Duration} Fps:{animation.Fps} InPoint: {animation.InPoint} OutPoint: {animation.OutPoint}");
        }
        else
        {
            Logger
                .TryGet(LogEventLevel.Warning, LogArea.Control)?
                .Log(this, "Failed to load animation.");
        }

        return animation;
    }

    private SkiaSharp.Skottie.Animation? Load(string path, Uri? baseUri)
    {
        var uri = path.StartsWith("/") 
            ? new Uri(path, UriKind.Relative) 
            : new Uri(path, UriKind.RelativeOrAbsolute);
        if (uri.IsAbsoluteUri && uri.IsFile)
        {
            using var fileStream = File.OpenRead(uri.LocalPath);
            return Load(fileStream);
        }

        using var assetStream = AssetLoader.Open(uri, baseUri);

        if (assetStream is null)
        {
            return default;
        }

        return Load(assetStream);
    }

    private void Load(string? path)
    {
        Stop();

        if (path is null)
        {
            DisposeImpl();
            return;
        }

        DisposeImpl();

        try
        {
            _repeatCount = RepeatCount;
            _playBackRate = PlayBackRate;
            _animation = Load(path, _baseUri);

            if (_animation is null)
            {
                return;
            }

            InvalidateArrange();
            InvalidateMeasure();

            if (AutoPlay)
            {
                Start();
            }
        }
        catch (Exception e)
        {
            Logger
                .TryGet(LogEventLevel.Warning, LogArea.Control)?
                .Log(this, "Failed to load animation: " + e);
            _animation = null;
        }
    }

    /// <summary>
    /// Starts or resumes the animation.
    /// </summary>
    public void Start()
    {
        _customVisual?.SendHandlerMessage(
            new LottiePayload(
                LottieCommand.Start,
                _animation,
                Stretch,
                StretchDirection,
                _repeatCount,
                _playBackRate,
                OnAnimationCompleted: OnAnimationCompleted,
                OnAnimationCompletedRepetition: OnAnimationCompletedRepetition));
    }

    /// <summary>
    /// Stops the animation.
    /// </summary>
    public void Stop()
    {
        _customVisual?.SendHandlerMessage(new LottiePayload(LottieCommand.Stop));
    }

    /// <summary>
    /// Pauses the animation.
    /// </summary>
    public void Pause()
    {
        _customVisual?.SendHandlerMessage(new LottiePayload(LottieCommand.Pause));
    }

    /// <summary>
    /// Resumes the animation if paused.
    /// </summary>
    public void Resume()
    {
        _customVisual?.SendHandlerMessage(new LottiePayload(LottieCommand.Resume));
    }

    /// <summary>
    /// Seeks to a given frame in the animation.
    /// </summary>
    public void SeekToFrame(float frame)
    {
        _customVisual?.SendHandlerMessage(new LottiePayload(LottieCommand.Seek, SeekFrame: frame));
    }

    /// <summary>
    /// Seeks to a given progress value between 0 and 1.
    /// </summary>
    public void SeekToProgress(float progress)
    {
        _customVisual?.SendHandlerMessage(new LottiePayload(LottieCommand.Seek, SeekProgress: progress));
    }

    private void OnAnimationCompleted()
    {
        AnimationCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void OnAnimationCompletedRepetition(int repetition)
    {
        AnimationCompletedRepetition?.Invoke(this, repetition);
    }

    private void DisposeImpl()
    {
        _customVisual?.SendHandlerMessage(new LottiePayload(LottieCommand.Dispose));
    }
}
