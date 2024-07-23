using System;
using System.IO;
using System.Numerics;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Gif;

public class GifImage : Control
{
    public static readonly StyledProperty<Uri> SourceUriProperty =
        AvaloniaProperty.Register<GifImage, Uri>("SourceUri");

    public static readonly StyledProperty<Stream> SourceStreamProperty =
        AvaloniaProperty.Register<GifImage, Stream>("SourceStream");

    public static readonly StyledProperty<IterationCount> IterationCountProperty =
        AvaloniaProperty.Register<GifImage, IterationCount>("IterationCount", IterationCount.Infinite);

    private GifInstance? _gifInstance;

    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<GifImage, StretchDirection>("StretchDirection");

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<GifImage, Stretch>("Stretch");

    private CompositionCustomVisual? _customVisual;

    private object? _initialSource;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        switch (change.Property.Name)
        {
            case nameof(SourceUri):
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

    public Uri SourceUri
    {
        get => GetValue(SourceUriProperty);
        set => SetValue(SourceUriProperty, value);
    }

    public Stream SourceStream
    {
        get => GetValue(SourceStreamProperty);
        set => SetValue(SourceStreamProperty, value);
    }

    public IterationCount IterationCount
    {
        get => GetValue(IterationCountProperty);
        set => SetValue(IterationCountProperty, value);
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

    private static void IterationCountChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var image = e.Sender as GifImage;
        if (image is null || e.NewValue is not IterationCount iterationCount)
            return;

        image.IterationCount = iterationCount;
    }

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

    /// <summary>
    /// Measures the control.
    /// </summary>
    /// <param name="availableSize">The available size.</param>
    /// <returns>The desired size of the control.</returns>
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