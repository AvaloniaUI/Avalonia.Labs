using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Controls;

[TemplatePart("PART_Shape", typeof(Avalonia.Controls.Shapes.Path))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
public class InfoBadge : ContentControl
{
    private readonly static EventHandler<VisualTreeAttachmentEventArgs> Parent_DetachedFromVisualTreeHandler = OnParent_DetachedFromVisualTree;
    private IDisposable? _disposablePadding;

    static InfoBadge()
    {
        TemplateProperty.OverrideDefaultValue<InfoBadge>(
            new FuncControlTemplate((_, ns) =>
            {
                return new Panel()
                {
                    VerticalAlignment = Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Layout.HorizontalAlignment.Center,
                    Children =
                    {
                        new Avalonia.Controls.Shapes.Path
                            {
                                VerticalAlignment = Layout.VerticalAlignment.Center,
                                HorizontalAlignment = Layout.HorizontalAlignment.Center,
                                Name = "PART_Shape",
                                [~Avalonia.Controls.Shapes.Path.DataProperty] = new TemplateBinding(ShapeProperty),
                                [~Avalonia.Controls.Shapes.Shape.StrokeProperty] = new TemplateBinding(BorderBrushProperty),
                                [~Avalonia.Controls.Shapes.Shape.StrokeThicknessProperty] = new TemplateBinding(BorderThicknessProperty),
                                [~Avalonia.Controls.Shapes.Shape.FillProperty] = new TemplateBinding(BackgroundProperty),
                            }.RegisterInNameScope(ns),
                        new ContentPresenter
                        {
                            Name = "PART_ContentPresenter",
                            HorizontalAlignment = Layout.HorizontalAlignment.Center,
                            VerticalAlignment = Layout.VerticalAlignment.Center,
                            [~MarginProperty] = new TemplateBinding(PaddingProperty),
                            [~ContentTemplateProperty] = new TemplateBinding(ContentTemplateProperty),
                            [~ContentProperty] = new TemplateBinding(ContentProperty),
                            [~PaddingProperty] = new TemplateBinding(PaddingProperty),
                            [~VerticalContentAlignmentProperty] = new TemplateBinding(VerticalContentAlignmentProperty),
                            [~HorizontalContentAlignmentProperty] = new TemplateBinding(HorizontalContentAlignmentProperty),
                        }.RegisterInNameScope(ns),
                    }
                };
            }));
        BadgeProperty.Changed.AddClassHandler<Visual>(OnBadgeChanged);
    }

    public static readonly AttachedProperty<InfoBadge?> BadgeProperty =
        AvaloniaProperty.RegisterAttached<InfoBadge, Visual, InfoBadge?>("Badge");

    /// <summary>
    /// Shape StyledProperty Definition
    /// </summary>
    public static readonly StyledProperty<Geometry> ShapeProperty =
        AvaloniaProperty.Register<InfoBadge, Geometry>(nameof(Shape));

    /// <summary>
    /// Get/Set the shape of <see cref="InfoBadge"/>
    /// </summary>
    public Geometry Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public static readonly StyledProperty<bool> AutoPaddingProperty =
        AvaloniaProperty.Register<InfoBadge, bool>(nameof(AutoPadding), true);

    public bool AutoPadding
    {
        get => GetValue(AutoPaddingProperty);
        set => SetValue(ShapeProperty, value);
    }

    /// <summary>
    /// ShowOnNullContent StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<bool> ShowOnNullContentProperty =
        AvaloniaProperty.Register<InfoBadge, bool>(nameof(ShowOnNullContent));

    /// <summary>
    /// Gets or sets the ShowOnNullContent property. This StyledProperty
    /// indicates ....
    /// </summary>
    public bool ShowOnNullContent
    {
        get => GetValue(ShowOnNullContentProperty);
        set => SetValue(ShowOnNullContentProperty, value);
    }

    public static InfoBadge? GetBadge(Visual element) =>
        element.GetValue(BadgeProperty);

    public static void SetBadge(Visual element, InfoBadge? value) =>
        element.SetValue(BadgeProperty, value);

    private static void OnBadgeChanged(Visual visual, AvaloniaPropertyChangedEventArgs change)
    {
        // If is Attached to VisualTree
        if (visual.IsAttachedToVisualTree())
        {
            if (AdornerLayer.GetAdornerLayer(visual) is { } layer)
            {
                if (change.OldValue is InfoBadge oldBadge)
                {
                    visual.DetachedFromVisualTree -= Parent_DetachedFromVisualTreeHandler;
                    AdornerLayer.SetAdornedElement(oldBadge, default!);
                    oldBadge.SetCurrentValue(DataContextProperty, null);
                    layer.Children.Remove(oldBadge);
                    oldBadge._disposablePadding?.Dispose();
                    oldBadge._disposablePadding = null;
                }
                if (change.NewValue is InfoBadge newBadge)
                {
                    newBadge.SetCurrentValue(DataContextProperty, visual.DataContext);
                    AdornerLayer.SetAdornedElement(newBadge, visual);
                    layer.Children.Add(newBadge);
                    visual.DetachedFromVisualTree += Parent_DetachedFromVisualTreeHandler;
                }
            }
        }
        else
        {
            EventHandler<VisualTreeAttachmentEventArgs>? loadHandler = default;
            loadHandler = (sender, e) =>
            {
                visual.AttachedToVisualTree -= loadHandler;
                Threading.Dispatcher.UIThread.Post(() =>
                    OnBadgeChanged(visual, change));
            };
            visual.AttachedToVisualTree += loadHandler;
        }
    }

    private static void OnParent_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is Visual visual)
        {
            visual.DetachedFromVisualTree -= Parent_DetachedFromVisualTreeHandler;
            if (AdornerLayer.GetAdornerLayer(e.Parent) is { } layer)
            {
                var badges = layer.Children.OfType<InfoBadge>().ToArray();
                foreach (var badge in badges)
                {
                    if (AdornerLayer.GetAdornedElement(visual) is null)
                    {
                        AdornerLayer.SetAdornedElement(badge, default!);
                        badge.SetCurrentValue(DataContextProperty, null);
                        layer.Children.Remove(badge);
                    }
                }
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty)
        {
            // Hide Adorner if Content is Null
            SetCurrentValue(IsVisibleProperty, ShowOnNullContent || change.NewValue is not null);
        }
        if (change.Property == IsVisibleProperty)
        {
            if (change.NewValue is false)
            {
                _disposablePadding?.Dispose();
                _disposablePadding = null;
            }
            else if (change.NewValue is true)
            {
                ApplyPadding();
            }
        }
        if (change.Property == ShowOnNullContentProperty)
        {
            SetCurrentValue(IsVisibleProperty, change.GetNewValue<bool>() || Content is not null);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        // Ensure Visibilty
        SetCurrentValue(IsVisibleProperty, ShowOnNullContent || Content is not null);
        _disposablePadding = ApplyPadding();
    }

    private IDisposable? ApplyPadding()
    {
        if (AutoPadding && AdornerLayer.GetAdornedElement(this) is Visual adorner
            && AvaloniaPropertyRegistry.Instance.IsRegistered(adorner, PaddingProperty))
        {
            var padding = adorner.GetValue(PaddingProperty);
            return adorner.SetValue(PaddingProperty, padding + GetPadding(adorner.Bounds.Size)
                , BindingPriority.Animation);
        }
        return default;
    }

    private Thickness GetPadding(Size availableSize)
    {
        double left = 0, top = 0, right = 0, bottom = 0;
        if (!IsMeasureValid)
        {
            Measure(availableSize);
        }
        if (HorizontalAlignment == Layout.HorizontalAlignment.Left)
        {
            left = DesiredSize.Width;
        }
        else if (HorizontalAlignment == Layout.HorizontalAlignment.Right)
        {
            right = DesiredSize.Width;
        }
        else if (HorizontalAlignment == Layout.HorizontalAlignment.Center && VerticalAlignment == Layout.VerticalAlignment.Top)
        {
            top = DesiredSize.Height;
        }
        else if (HorizontalAlignment == Layout.HorizontalAlignment.Center && VerticalAlignment == Layout.VerticalAlignment.Bottom)
        {
            bottom = DesiredSize.Height;
        }
        return new(left, top, right, bottom);
    }
}

