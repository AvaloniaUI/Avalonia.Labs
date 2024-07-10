using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Labs.Notifications.Linux
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder WithDBusAppNotifications(this AppBuilder appBuilder, DBusNotificationOptions options)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return appBuilder;

            var notificationManager = new LinuxNativeNotificationManager();
            NativeNotificationManager.RegisterNativeNotificationManager(notificationManager);

            if (options.Channels is not null)
            {
                foreach (var channel in options.Channels)
                {
                    notificationManager.ChannelManager.AddChannel(channel);
                }
            }

            var callback = appBuilder.AfterSetupCallback;
            callback += (a) =>
            {
                notificationManager.Initialize();
                var lifetime = Application.Current?.ApplicationLifetime;

                if (lifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
                {
                    desktopStyleApplicationLifetime.ShutdownRequested += (s, e) =>
                    {
                        notificationManager.Dispose();
                    };
                }
            };

            appBuilder.AfterSetup(callback);

            return appBuilder;
        }
    }

    public class DBusNotificationOptions
    {
        public IReadOnlyList<NotificationChannel>? Channels { get; set; }
    }
}
