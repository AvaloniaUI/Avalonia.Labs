using System;
using Avalonia.Controls.Notifications;

namespace Avalonia.Labs.Controls;

// INotificationManager.cs
public interface INotificationManager
{
    void ShowNotification(NotificationOptions options);
    NotificationPosition DefaultNotificationPosition { get; set; }
    TimeSpan DefaultDuration { get; set; }
}
