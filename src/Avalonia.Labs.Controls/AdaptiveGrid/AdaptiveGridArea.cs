// Port of https://github.com/Marplex/AdaptiveGrid 
// Commit Id: 053b17c0cc8d4d586a673c860183ea1836693c70

namespace Avalonia.Labs.Controls;

/// <summary>
/// Defines a <see cref="AdaptiveGrid"/> area
/// </summary>
public class AdaptiveGridArea : StyledElement
{
    /// <summary>
    /// Property for <see cref="Area"/>.
    /// </summary>
    public static readonly StyledProperty<string?> AreaProperty =
        AvaloniaProperty.Register<AdaptiveGridArea, string?>(nameof(Area));


    /// <summary>
    /// Property for <see cref="Column"/>.
    /// </summary>
    public static readonly StyledProperty<int> ColumnProperty =
        AvaloniaProperty.Register<AdaptiveGridArea, int>(nameof(Column));

    /// <summary>
    /// Property for <see cref="Row"/>.
    /// </summary>
    public static readonly StyledProperty<int> RowProperty =
        AvaloniaProperty.Register<AdaptiveGridArea, int>(nameof(Row));

    /// <summary>
    /// Property for <see cref="ColumnSpan"/>.
    /// </summary>
    public static readonly StyledProperty<int> ColumnSpanProperty =
        AvaloniaProperty.Register<AdaptiveGridArea, int>(nameof(ColumnSpan), 1);

    /// <summary>
    /// Property for <see cref="RowSpan"/>.
    /// </summary>
    public static readonly StyledProperty<int> RowSpanProperty =
        AvaloniaProperty.Register<AdaptiveGridArea, int>(nameof(RowSpan), 1);


    /// <summary>
    /// Set or get the area name
    /// </summary>
    public string? Area
    {
        get => GetValue(AreaProperty);
        set => SetValue(AreaProperty, value);
    }

    /// <summary>
    /// Set or get the area column position
    /// </summary>
    public int Column
    {
        get => GetValue(ColumnProperty);
        set => SetValue(ColumnProperty, value);
    }

    /// <summary>
    /// Set or get the area row position
    /// </summary>
    public int Row
    {
        get => GetValue(RowProperty);
        set => SetValue(RowProperty, value);
    }

    /// <summary>
    /// Set or get the area column span
    /// </summary>
    public int ColumnSpan
    {
        get => GetValue(ColumnSpanProperty);
        set => SetValue(ColumnSpanProperty, value);
    }

    /// <summary>
    /// Set or get the area row span
    /// </summary>
    public int RowSpan
    {
        get => GetValue<int>(RowSpanProperty);
        set => SetValue(RowSpanProperty, value);
    }

}
