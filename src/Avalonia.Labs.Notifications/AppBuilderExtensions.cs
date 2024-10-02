using System;
using Avalonia.Labs.Notifications.Apple;
using Avalonia.Threading;

namespace Avalonia.Labs.Notifications
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder WithAppNotifications(this AppBuilder appBuilder
#if ANDROID
            , global::Android.App.Activity activity
#endif
            , AppNotificationOptions? options = null
            )
        {
            INativeNotificationManagerImpl notificationManager;
#if ANDROID
            notificationManager = new Android.NativeNotificationManager(activity);
#else
            if (OperatingSystem.IsMacOS() || OperatingSystem.IsIOS())
            {
                if (!OperatingSystem.IsMacOSVersionAtLeast(10, 14) && !OperatingSystem.IsIOSVersionAtLeast(10))
                    return appBuilder;

                var identifier = AppleInterop.Bundle.GetMainBundleIdentifier();
                if (identifier is null)
                    return appBuilder;

                notificationManager = new AppleNativeNotificationManager(identifier);
            }
#if INCLUDE_WINDOWS
            else if (OperatingSystem.IsWindows())
            {
                if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
                    return appBuilder;

                notificationManager = new Windows.NativeNotificationManager();
            }
#endif
#if INCLUDE_LINUX
            else if (OperatingSystem.IsLinux())
            {
                notificationManager = new Linux.LinuxNativeNotificationManager();
            }
#endif
            else
            {
                return appBuilder;
            }
#endif
            NativeNotificationManager.RegisterNativeNotificationManager(notificationManager);

            foreach (var channel in options?.Channels ?? [])
            {
                notificationManager.ChannelManager.AddChannel(channel);
            }

            var callback = appBuilder.AfterSetupCallback;
            callback += (a) =>
            {
                notificationManager.Initialize(options);
                Dispatcher.UIThread.ShutdownStarted += (sender, args) =>
                {
                    notificationManager.Dispose();
                };
            };

            appBuilder.AfterSetup(callback);

            return appBuilder;
        }
    }
}
