using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Labs.Notifications
{
    public static partial class AppBuilderExtensions
    {
        public static AppBuilder WithWin32AppNotifications(this AppBuilder appBuilder, Win32NotificationOptions options)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
                return appBuilder;

            var notificationManager = new Avalonia.Labs.Notifications.Windows.NativeNotificationManager();
            Notifications.NativeNotificationManager.RegisterNativeNotificationManager(notificationManager);

            if (options.Channels != null)
                foreach (var channel in options.Channels)
                {
                    notificationManager.ChannelManager.AddChannel(channel);
                }
            
            appBuilder.AfterSetup(_ =>
            {
                notificationManager.Initialize(options);
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
        /// <summary>
        /// Application display name for notifications.
        /// If not defined, Application.Name if used.
        /// Is ignored for packaged applications.
        /// </summary>
        public string? AppName { get; set; }

        /// <summary>
        /// Application icon for notifications.
        /// Is ignored for packaged applications.
        /// </summary>
        public string? AppIcon { get; set; }

        /// <summary>
        /// Overrides AppUserModelId used for notifications.
        /// Is ignored for packged applications.
        /// </summary>
        public string? AppUserModelId { get; set; }

        public IReadOnlyList<NotificationChannel>? Channels { get; set; }
    }
}
