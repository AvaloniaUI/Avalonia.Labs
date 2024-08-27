using System;
using System.IO;
using System.Numerics;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Gif;

/// <summary>
/// An Avalonia control that allows GIF playback. 
/// </summary>
public class GifImage : Control
{
    /// <summary>
    /// Defines the <see cref="Source"/> property.
    /// </summary>
    public static readonly StyledProperty<Uri> SourceProperty =
        AvaloniaProperty.Register<GifImage, Uri>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="SourceStream"/> property.
    /// </summary>
    public static readonly StyledProperty<Stream> SourceStreamProperty =
        AvaloniaProperty.Register<GifImage, Stream>(nameof(SourceStream));

    /// <summary>
    /// Defines the <see cref="IterationCount"/> property.
    /// </summary>
    public static readonly StyledProperty<IterationCount> IterationCountProperty =
        AvaloniaProperty.Register<GifImage, IterationCount>(nameof(IterationCount), IterationCount.Infinite);

    /// <summary>
    /// Defines the <see cref="StretchDirection"/> property.
    /// </summary>
    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<GifImage, StretchDirection>(nameof(StretchDirection));

    /// <summary>
    /// Defines the <see cref="Stretch"/> property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<GifImage, Stretch>(nameof(Stretch));

    private GifInstance? _gifInstance;
    
    private CompositionCustomVisual? _customVisual;

    private object? _initialSource;

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        switch (change.Property.Name)
        {
            case nameof(Source):
            case nameof(SourceStream):
                SourceChanged(change);
                break;
            case nameof(Stretch):
            case nameof(StretchDirection):
                InvalidateArrange();
                InvalidateMeasure();
                Update();
                break;
            case nameof(IterationCount):
                IterationCountChanged(change);
                break;
            case nameof(Bounds):
                Update();
                break;
        }

        base.OnPropertyChanged(change);
    }

    /// <summary>
    /// Gets or sets the uri pointing to the GIF image resource
    /// </summary>
    public Uri Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the stream pointing to the GIF image resource
    /// </summary>
    public Stream SourceStream
    {
        get => GetValue(SourceStreamProperty);
        set => SetValue(SourceStreamProperty, value);
    }

    /// <summary>
    /// Gets or sets the amount in which the GIF image loops.
    /// </summary>
    public IterationCount IterationCount
    {
        get => GetValue(IterationCountProperty);
        set => SetValue(IterationCountProperty, value);
    }


    /// <summary>
    /// Gets or sets a value controlling how the image will be stretched.
    /// </summary>
    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Gets or sets a value controlling in what direction the image will be stretched.
    /// </summary>
    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }

    private static void IterationCountChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var image = e.Sender as GifImage;
        if (image is null || e.NewValue is not IterationCount iterationCount)
            return;

        image.IterationCount = iterationCount;
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var compositor = ElementComposition.GetElementVisual(this)?.Compositor;
        if (compositor == null || _customVisual?.Compositor == compositor)
            return;
        _customVisual = compositor.CreateCustomVisual(new GifCustomVisualHandler());
        ElementComposition.SetElementChildVisual(this, _customVisual);
        _customVisual.SendHandlerMessage(GifCustomVisualHandler.StartMessage);

        if (_initialSource is not null)
        {
            UpdateGifInstance(_initialSource);
            _initialSource = null;
        }

        Update();
        base.OnAttachedToVisualTree(e);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var compositor = ElementComposition.GetElementVisual(this)?.Compositor;
        if (compositor == null || _customVisual?.Compositor == compositor)
            return;
            
        ElementComposition.SetElementChildVisual(this, null);
        _customVisual?.SendHandlerMessage(GifCustomVisualHandler.StopMessage);
        _customVisual = null;
    }


    private void Update()
    {
        if (_customVisual is null || _gifInstance is null)
            return;

        var dpi = this.GetVisualRoot()?.RenderScaling ?? 1.0;
        var sourceSize = _gifInstance.GifPixelSize.ToSize(dpi);
        var viewPort = new Rect(Bounds.Size);

        var scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection);
        var scaledSize = sourceSize * scale;
        var destRect = viewPort
            .CenterRect(new Rect(scaledSize))
            .Intersect(viewPort);

        _customVisual.Size = Stretch == Stretch.None ?
            new Vector2((float)sourceSize.Width, (float)sourceSize.Height) : 
            new Vector2((float)destRect.Size.Width, (float)destRect.Size.Height);

        _customVisual.Offset = new Vector3((float)destRect.Position.X, (float)destRect.Position.Y, 0);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        var result = new Size();
        var scaling = this.GetVisualRoot()?.RenderScaling ?? 1.0;
        if (_gifInstance != null)
        {
            result = Stretch.CalculateSize(availableSize, _gifInstance.GifPixelSize.ToSize(scaling),
                StretchDirection);
        }

        return result;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_gifInstance is null) return new Size();
        var scaling = this.GetVisualRoot()?.RenderScaling ?? 1.0;
        var sourceSize = _gifInstance.GifPixelSize.ToSize(scaling);
        var result = Stretch.CalculateSize(finalSize, sourceSize);
        return result;
    }

    private void SourceChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is null ||
            (e.NewValue is string value && !Uri.IsWellFormedUriString(value, UriKind.Absolute)))
        {
            return;
        }

        if (_customVisual is null)
        {
            _initialSource = e.NewValue;
            return;
        }

        UpdateGifInstance(e.NewValue);

        InvalidateArrange();
        InvalidateMeasure();
        Update();
    }

    private void UpdateGifInstance(object source)
    {
        _gifInstance?.Dispose();
        _gifInstance = new GifInstance(source);
        _gifInstance.IterationCount = IterationCount;
        _customVisual?.SendHandlerMessage(_gifInstance);
    }
}
