using System.Collections.Generic;

#if ANDROID
namespace Avalonia.Labs.Notifications.Android
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder WithAndroidNotifications(this AppBuilder appBuilder, AndroidNotificationOptions options, Activity activity)
        {
            var notificationManager = new NativeNotificationManager(activity);
            Notifications.NativeNotificationManager.RegisterNativeNotificationManager(notificationManager);

            foreach (var channel in options.Channels)
            {
                notificationManager.ChannelManager.AddChannel(channel);
            }

            var callback = appBuilder.AfterSetupCallback;
            callback += (a) =>
            {
                notificationManager.Initialize();
            };

            appBuilder.AfterSetup(callback);

            return appBuilder;
        }
    }

    public class AndroidNotificationOptions
    {
        public IReadOnlyList<NotificationChannel> Channels { get; init; }
    }
}
#endif
