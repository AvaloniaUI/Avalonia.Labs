using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Labs.Notifications.Linux;

namespace Avalonia.Labs.Notifications
{
    public static partial class AppBuilderExtensions
    {
        public static AppBuilder WithX11AppNotifications(this AppBuilder appBuilder, X11NotificationOptions options)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return appBuilder;

            var notificationManager = new LinuxNativeNotificationManager(options.AppName ?? "", options.AppIcon);
            NativeNotificationManager.RegisterNativeNotificationManager(notificationManager);

            if (options.Channels != null)
                foreach (var channel in options.Channels)
                {
                    notificationManager.ChannelManager.AddChannel(channel);
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

    public class X11NotificationOptions
    {
        public string? AppName { get; set; }
        public IReadOnlyList<NotificationChannel>? Channels { get; set; }
        public string? AppIcon { get; set; }
    }
}
