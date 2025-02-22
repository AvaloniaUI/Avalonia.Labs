using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace Avalonia.Labs.Controls;

[TemplatePart(s_tpItemsControlTopLeft, typeof(ItemsControl))]
[TemplatePart(s_tpItemsControlTopCenter, typeof(ItemsControl))]
[TemplatePart(s_tpItemsControlTopRight, typeof(ItemsControl))]
[TemplatePart(s_tpItemsControlBottomLeft, typeof(ItemsControl))]
[TemplatePart(s_tpItemsControlBottomCenter, typeof(ItemsControl))]
[TemplatePart(s_tpItemsControlBottomRight, typeof(ItemsControl))]
public partial class NotificationHost
{
    public static readonly StyledProperty<int> MinimumDesktopWidthProperty =
        AvaloniaProperty.Register<Controls.NotificationHost, int>(
            nameof(MinimumDesktopWidth),
            500);

    public int MinimumDesktopWidth
    {
        get => GetValue(MinimumDesktopWidthProperty);
        set => SetValue(MinimumDesktopWidthProperty, value);
    }
    
    
    private const string s_tpItemsControlTopLeft = "PART_TopLeftNotifications";
    private const string s_tpItemsControlTopCenter = "PART_TopCenterNotifications";
    private const string s_tpItemsControlTopRight = "PART_TopRightNotifications";
    private const string s_tpItemsControlBottomLeft = "PART_BottomLeftNotifications";
    private const string s_tpItemsControlBottomCenter = "PART_BottomCenterNotifications";
    private const string s_tpItemsControlBottomRight = "PART_BottomRightNotifications";
}
