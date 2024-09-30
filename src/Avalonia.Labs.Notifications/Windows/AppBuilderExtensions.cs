using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Labs.Notifications.Windows
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder WithWin32AppNotifications(this AppBuilder appBuilder, Win32NotificationOptions options)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
                return appBuilder;
            var notificationManager = new NativeNotificationManager();
            Notifications.NativeNotificationManager.RegisterNativeNotificationManager(notificationManager);

            if (options.Channels != null)
                foreach (var channel in options.Channels)
                {
                    notificationManager.ChannelManager.AddChannel(channel);
                }
            
            appBuilder.AfterSetup(_ =>
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
            });

            return appBuilder;
        }
    }

    public class Win32NotificationOptions
    {
        public IReadOnlyList<NotificationChannel>? Channels { get; init; }
    }
}
