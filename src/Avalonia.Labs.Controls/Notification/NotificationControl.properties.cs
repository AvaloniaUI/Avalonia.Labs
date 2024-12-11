using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Presenters;

namespace Avalonia.Labs.Controls;

[TemplatePart(s_tpLayoutRoot, typeof(Panel))]
[TemplatePart(s_tpNotificationBorder, typeof(Border))]
[TemplatePart(s_tpNotificationPresenter, typeof(ContentPresenter))]
[TemplatePart(s_tpActionButton, typeof(Button))]
public partial class NotificationControl
{
    public static readonly StyledProperty<object?> ActionButtonContentProperty =
        AvaloniaProperty.Register<NotificationControl, object?>(nameof(Type));
    
    public static readonly StyledProperty<NotificationType> TypeProperty =
        AvaloniaProperty.Register<NotificationControl, NotificationType>(nameof(Type));
    
    public object? ActionButtonContent
    {
        get => GetValue(ActionButtonContentProperty);
        set => SetValue(ActionButtonContentProperty, value);
    }
    
    public NotificationType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }
    
    public event EventHandler? Closed;
    
    
    private const string s_tpLayoutRoot = "PART_LayoutRoot";
    private const string s_tpNotificationPresenter = "PART_NotificationPresenter";
    private const string s_tpActionButton = "PART_ActionButton";
    private const string s_tpNotificationBorder = "PART_NotificationBorder";
    
}
