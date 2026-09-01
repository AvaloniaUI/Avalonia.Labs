using System;
using System.Collections.Generic;
#if INCLUDE_WINDOWS
using System.Diagnostics;
#endif
using System.IO;
using Avalonia.Labs.Controls.Cache;
using Avalonia.Labs.Notifications;

namespace Avalonia.Labs.Catalog.Desktop;

sealed class Program
{
    private static NotificationChannel[] s_channels = new[]
    {
        new NotificationChannel("basic", "Send Notifications", NotificationPriority.High),
        new NotificationChannel("actions", "Send Notification with Predefined Actions", NotificationPriority.High)
        {
            Actions = new List<NativeNotificationAction>
            {
                new("Hello", "hello"),
                new("world", "world")
            }
        },
        new NotificationChannel("custom", "Send Notification with Custom Actions", NotificationPriority.High),
        new NotificationChannel("reply", "Send Notification with Reply Action", NotificationPriority.High)
        {
            Actions = [new NativeNotificationAction("Reply", "reply")]
        },
    };

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .WithAppNotifications(new AppNotificationOptions()
            {
                Channels = s_channels,
#if INCLUDE_WINDOWS
                AppIcon = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? AppContext.BaseDirectory, "avalonia-32.png"),
                AppName = "Avalonia.Labs",
                AppUserModelId = "com.Avalonia.Labs.Catalog",
                // Is required for Packaged project, optional for the rest.
                // ComActivatorGuidOverride = Guid.Parse("67890354-2A47-444C-B15F-DBF513C82F03")
#endif
            })
            .AfterSetup(builder =>
            {
                CacheOptions.SetDefault(new CacheOptions()
                {
                    BaseCachePath = Path.Combine(Path.GetTempPath(), "Avalonia.Labs")
                });
            });
}
