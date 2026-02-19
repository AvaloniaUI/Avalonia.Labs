using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Avalonia.Android;
using Avalonia.Labs.Notifications;
using NotificationChannel = Avalonia.Labs.Notifications.NotificationChannel;

namespace Avalonia.Labs.Catalog.Android
{
    [Application]
    public class Application : AvaloniaAndroidApplication<App>
    {
        protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            return base.CustomizeAppBuilder(builder)
                .WithAppNotifications(this, new AppNotificationOptions()
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
}
