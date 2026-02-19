using System.Collections.Generic;
using Foundation;
using Avalonia.iOS;
using Avalonia.Labs.Notifications;

namespace Avalonia.Labs.Catalog.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public partial class AppDelegate : AvaloniaAppDelegate<App>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithAppNotifications(new AppNotificationOptions()
            {
                Channels = new[]
                {
                    new NotificationChannel("basic", "Send Notifications", Notifications.NotificationPriority.High),
                    new NotificationChannel("actions", "Send Notification with Predefined Actions", Notifications.NotificationPriority.High)
                    {
                        Actions = new List<NativeNotificationAction>
                        {
                            new NativeNotificationAction("Hello", "hello"),
                            new NativeNotificationAction("world", "world")
                        }
                    },
                    new NotificationChannel("custom", "Send Notification with Custom Actions", Notifications.NotificationPriority.High),
                    new NotificationChannel("reply", "Send Notification with Reply Action", Notifications.NotificationPriority.High)
                    {
                        Actions = new List<NativeNotificationAction>
                        {
                            new NativeNotificationAction("Reply", "reply")
                        }
                    },
                }
            });
    }
}
