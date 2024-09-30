using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Labs.Controls.Cache;
using Avalonia.Labs.Notifications;
using Avalonia.Labs.Notifications.Windows;
using Avalonia.Labs.Notifications.Linux;
using Avalonia.Platform;
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
            .WithX11AppNotifications(new X11NotificationOptions()
            {
                AppIcon = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/avalonia-32.png",
                AppName = "Avalonia.Labs",
                Channels = new[]
                    {
                        new NotificationChannel("basic", "Send Notifications", Notifications.NotificationPriority.High),
                        new NotificationChannel("actions", "Send Notification with Predefined Actions", Notifications.NotificationPriority.High)
                        {
                            Actions = new List<NativeNotificationAction>
                            {
                                new NativeNotificationAction()
                                {
                                    Tag = "hello",
                                    Caption = "Hello"
                                },
                                new NativeNotificationAction()
                                {
                                    Tag = "world",
                                    Caption = "world"
                                }
                            }
                        },
                        new NotificationChannel("custom", "Send Notification with Custom Actions", Notifications.NotificationPriority.High),
                        new NotificationChannel("reply", "Send Notification with Reply Action", Notifications.NotificationPriority.High)
                        {
                            Actions = new List<NativeNotificationAction>
                            {
                                new NativeNotificationAction()
                                {
                                    Tag = "reply",
                                    Caption = "Reply"
                                }
                            }
                        },
                    }
            })
            .WithWin32AppNotifications(new Win32NotificationOptions()
            {
                AppName = "Avalonia.Labs",
                AppIcon = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/avalonia-32.png",
                AppUserModelId = "com.Avalonia.Labs.Catalog",
                Channels = new[]
                    {
                        new NotificationChannel("basic", "Send Notifications", Notifications.NotificationPriority.High),
                        new NotificationChannel("actions", "Send Notification with Predefined Actions", Notifications.NotificationPriority.High)
                        {
                            Actions = new List<NativeNotificationAction>
                            {
                                new NativeNotificationAction()
                                {
                                    Tag = "hello",
                                    Caption = "Hello"
                                },
                                new NativeNotificationAction()
                                {
                                    Tag = "world",
                                    Caption = "world"
                                }
                            }
                        },
                        new NotificationChannel("custom", "Send Notification with Custom Actions", Notifications.NotificationPriority.High),
                        new NotificationChannel("reply", "Send Notification with Reply Action", Notifications.NotificationPriority.High)
                        {
                            Actions = new List<NativeNotificationAction>
                            {
                                new NativeNotificationAction()
                                {
                                    Tag = "reply",
                                    Caption = "Reply"
                                }
                            }
                        },
                    }
            })
            .AfterSetup(builder =>
            {
                CacheOptions.SetDefault(new CacheOptions()
                {
                    BaseCachePath = Path.Combine(Path.GetTempPath(), "Avalonia.Labs")
                });
            })
            .LogToTrace();
}
