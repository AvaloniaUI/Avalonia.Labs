using AppleInterop;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Labs.Notifications.Apple;

namespace Avalonia.Labs.Notifications;

public partial class AppBuilderExtensions
{
    public static AppBuilder WithAppleAppNotifications(this AppBuilder appBuilder, AppleNotificationOptions options)
    {
        if (!OperatingSystem.IsMacOSVersionAtLeast(10, 14) && !OperatingSystem.IsIOSVersionAtLeast(10))
            return appBuilder;

        var identifier = Bundle.GetMainBundleIdentifier();
        if (identifier is null)
            return appBuilder;

        var notificationManager = new AppleNativeNotificationManager(identifier);
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

public class AppleNotificationOptions
{
    public IReadOnlyList<NotificationChannel> Channels { get; init; }
}
