using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Avalonia.Labs.Controls.Cache;
using Avalonia.Labs.Notifications;
using Avalonia.Labs.Notifications.Linux;
using Avalonia.Labs.Notifications.Windows;
using Avalonia.ReactiveUI;


namespace Avalonia.Labs.Catalog.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Debugger.Launch();
        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .WithDBusAppNotifications(new DBusNotificationOptions
            {
                Channels =
                [
                    new NotificationChannel("basic", "Send Notifications", NotificationPriority.High),
                        new NotificationChannel("actions", "Send Notification with Predefined Actions", NotificationPriority.High)
                        {
                            Actions =
                            [
                                new NativeNotificationAction("hello", "Hello"),
                                new NativeNotificationAction("world", "world")
                            ]
                        },
                        new NotificationChannel("custom", "Send Notification with Custom Actions", NotificationPriority.High),
                        new NotificationChannel("reply", "Send Notification with Reply Action", NotificationPriority.High)
                        {
                            Actions = [new NativeNotificationAction("reply", "Reply")]
                        }
                ]
            })
            .WithWin32AppNotifications(new Win32NotificationOptions
            {
                Channels =
                [
                    new NotificationChannel("basic", "Send Notifications", NotificationPriority.High),
                    new NotificationChannel("actions", "Send Notification with Predefined Actions", NotificationPriority.High)
                    {
                        Actions =
                        [
                            new NativeNotificationAction("hello", "Hello"),
                            new NativeNotificationAction("world", "world")
                        ]
                    },
                    new NotificationChannel("custom", "Send Notification with Custom Actions", NotificationPriority.High),
                    new NotificationChannel("reply", "Send Notification with Reply Action", NotificationPriority.High)
                    {
                        Actions = [new NativeNotificationAction("reply", "Reply")]
                    }
                ]
            })
            .AfterSetup(builder =>
            {
                CacheOptions.SetDefault(new CacheOptions
                {
                    BaseCachePath = Path.Combine(Path.GetTempPath(), "Avalonia.Labs")
                });
            })
            .LogToTrace();
}
