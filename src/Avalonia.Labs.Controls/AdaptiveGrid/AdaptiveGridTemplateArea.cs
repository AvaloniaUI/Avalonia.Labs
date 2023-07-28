// Port of https://github.com/Marplex/AdaptiveGrid 
// Commit Id: 053b17c0cc8d4d586a673c860183ea1836693c70
using System.Collections.Generic;
using Avalonia;

namespace Avalonia.Labs.Controls;

/// <summary>
/// A collection of <see cref="AdaptiveGridArea"/>
/// </summary>
public class AdaptiveGridTemplateArea : StyledElement
{
    private List<AdaptiveGridArea>? _area;

    /// <summary>
    /// Property for <see cref="FromWidth"/>.
    /// </summary>
    public static readonly StyledProperty<double> FromWidthProperty =
        AvaloniaProperty.Register<AdaptiveGridTemplateArea, double>(nameof(FromWidth));

    /// <summary>
    /// Property for <see cref="Areas"/>.
    /// </summary>
    public static readonly DirectProperty<AdaptiveGridTemplateArea, IList<AdaptiveGridArea>> AreasProperty =
        AvaloniaProperty.RegisterDirect<AdaptiveGridTemplateArea, IList<AdaptiveGridArea>>(nameof(Areas),
            o => o.Areas
            );

    /// <summary>
    /// Set or get the minimum width to display this template
    /// </summary>
    public double FromWidth
    {
        get => GetValue<double>(FromWidthProperty);
        set => SetValue(FromWidthProperty, value);
    }

    /// <summary>
    /// Set or get the areas that define this template
    /// </summary>
    public IList<AdaptiveGridArea> Areas
    {
        get => _area ??= new();
    }
}
