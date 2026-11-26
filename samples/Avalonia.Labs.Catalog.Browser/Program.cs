using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Labs.Catalog;
using Avalonia.Labs.Notifications;

internal sealed partial class Program
{
    private static NotificationChannel[] s_channels = new[]
    {
        new NotificationChannel("basic", "Send Notifications", NotificationPriority.High)
        {
            Icon = "./favicon.ico",
        },
        new NotificationChannel("actions", "Send Notification with Predefined Actions", NotificationPriority.High)
        {
            Icon = "./favicon.ico",
            Actions = new List<NativeNotificationAction>
            {
                new("Hello", "hello"),
                new("world", "world")
            },
            Vibrations = [200,100,200],
        },
        new NotificationChannel("custom", "Send Notification with Custom Actions", NotificationPriority.High)
        {
            Icon = "./favicon.ico",
        },
        new NotificationChannel("reply", "Send Notification with Reply Action", NotificationPriority.High)
        {
            Icon = "./favicon.ico",
            Actions = [new NativeNotificationAction("Reply", "reply")]
        },
    };

    private static Task Main(string[] args) => BuildAvaloniaApp()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
        .WithAppNotifications(new AppNotificationOptions()
        {
            Channels = s_channels
        });
}
