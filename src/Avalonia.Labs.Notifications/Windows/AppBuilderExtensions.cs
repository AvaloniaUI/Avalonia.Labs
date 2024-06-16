using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Labs.Notifications.Windows
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder WithWin32AppNotifications(this AppBuilder appBuilder, Win32NotificationOptions options)
        {
            if (!OperatingSystem.IsWindows())
                return appBuilder;
            var notificationManager = new NativeNotificationManager();
            Notifications.NativeNotificationManager.RegisterNativeNotificationManager(notificationManager);

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

    public class Win32NotificationOptions
    {
        public IReadOnlyList<NotificationChannel>? Channels { get; init; }
    }
}
