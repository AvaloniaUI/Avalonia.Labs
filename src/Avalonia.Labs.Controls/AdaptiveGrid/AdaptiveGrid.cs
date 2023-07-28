// Port of https://github.com/Marplex/AdaptiveGrid 
// Commit Id: 053b17c0cc8d4d586a673c860183ea1836693c70
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;

namespace Avalonia.Labs.Controls;

/// <summary>
/// A responsive <see cref="Grid"/> alternative.
/// Change the layout distribution based on the grid's width.
/// Inspired by CSS grid
/// </summary>
public class AdaptiveGrid : Grid
{

    private AdaptiveGridTemplateArea? _latestGridTemplateArea = null;

    #region Properties
    /// <summary>
    /// Property for <see cref="TemplateAreas"/>.
    /// </summary>
    public static StyledProperty<IList<AdaptiveGridTemplateArea>> TemplateAreaProperty =
        AvaloniaProperty.Register<AdaptiveGrid, IList<AdaptiveGridTemplateArea>>(nameof(TemplateAreas));


    /// <summary>
    /// Template for grid areas
    /// </summary>
    public IList<AdaptiveGridTemplateArea> TemplateAreas
    {
        get => GetValue(TemplateAreaProperty);
        set => SetValue(TemplateAreaProperty, value);
    }

    /// <summary>
    /// Area name attached dependency property
    /// </summary>
    public static readonly AttachedProperty<string?> AreaProperty =
        AvaloniaProperty.RegisterAttached<AdaptiveGrid, Control, string?>("Area");

    public static string? GetArea(Control target) => target.GetValue(AreaProperty);

    public static void SetArea(StyledElement target, string? value) => target.SetValue(AreaProperty, value);
    #endregion

    public AdaptiveGrid()
    {
        TemplateAreas = new List<AdaptiveGridTemplateArea>();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty)
        {
            GridSizeChanged();
        }
        else if (change.Property == AreaProperty)
        {
            InvalidateVisual();
        }
    }

    private void GridSizeChanged()
    {
        //Responsive grid, change children distribution based on width
        var width = Bounds.Width;
        if (TemplateAreas is null)
        {
            return;
        }
        var templateArea = FindLast(TemplateAreas, (it, w) => w > it.FromWidth, width);
        if (templateArea == null || templateArea == _latestGridTemplateArea)
            return;

        _latestGridTemplateArea = templateArea;
        foreach (var area in templateArea.Areas)
        {
            var elements = Children.OfType<Control>()
                .Where(it => GetArea(it) == area.Area)
                .ToList();

            foreach (var element in elements)
            {

                //For every control that defined its "Area" property,
                //change the position on the grid
                SetColumn(element, area.Column);
                SetRow(element, area.Row);
                SetRowSpan(element, area.RowSpan);
                SetColumnSpan(element, area.ColumnSpan);
            }
        }

        static T? FindLast<T, TArg>(IList<T> source, Func<T, TArg, bool> predicate, TArg argument)
        {
            if (source?.Count is int count)
            {
                for (int i = count - 1; i > -1; i--)
                {
                    if (predicate(source[i], argument))
                    {
                        return source[i];
                    }
                }
            }
            return default(T);
        }
    }

}
