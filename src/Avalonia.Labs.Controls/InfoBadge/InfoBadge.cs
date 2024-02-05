using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Controls;

[TemplatePart("PART_Shape", typeof(Avalonia.Controls.Shapes.Path))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
public class InfoBadge : ContentControl
{
    private readonly static EventHandler<VisualTreeAttachmentEventArgs> Parent_DetachedFromVisualTreeHandler = OnParent_DetachedFromVisualTree;

    static InfoBadge()
    {
        TemplateProperty.OverrideDefaultValue<ContentControl>(
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
                                //StrokeThickness = 5,
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
                }
                if (change.NewValue is InfoBadge newBadge)
                {
                    newBadge.SetCurrentValue(DataContextProperty, visual.DataContext);
                    layer.Children.Add(newBadge);
                    AdornerLayer.SetAdornedElement(newBadge, visual);
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
            SetCurrentValue(IsVisibleProperty, change.NewValue is not null);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        // Ensure Visibilty
        SetCurrentValue(IsVisibleProperty, Content is not null);
    }
}

