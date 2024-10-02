using System.Runtime.Versioning;
using AppleInterop;
using Avalonia.Threading;

namespace Avalonia.Labs.Notifications.Apple;

[SupportedOSPlatform("ios")]
[SupportedOSPlatform("macos")]
internal class AppleNativeNotificationManager : INativeNotificationManager, IDisposable
{
    private readonly string _identifier;
    private readonly UNUserNotificationCenterDelegate _notificationDelegate;

    public AppleNativeNotificationManager(string identifier)
    {
        _identifier = identifier;
        _notificationDelegate = new UNUserNotificationCenterDelegate();
        ChannelManager = new AppleNotificationChannelManager();
    }

    public AppleNotificationChannelManager ChannelManager { get; }
    public IDictionary<uint, INativeNotification> ActiveNotifications { get; }

    public INativeNotification? CreateNotification(string? category)
    {
        var channel = ChannelManager?.GetChannel(category ?? NotificationChannelManager.DefaultChannel) ??
                      ChannelManager?.AddChannel(new NotificationChannel(NotificationChannelManager.DefaultChannel, NotificationChannelManager.DefaultChannelLabel));

        if (channel == null)
        {
            return null;
        }

        return new AppleNativeNotification(channel, _identifier, this);
    }

    public void CloseAll() => UNUserNotificationCenter.Current?.RemoveAllPending();

    public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

    public void Initialize()
    {
        var current = UNUserNotificationCenter.Current;
        current.Delegate = _notificationDelegate;
        ChannelManager.RegisterTo(current);
    }

    public void Dispose()
    {
        UNUserNotificationCenter.Current.Delegate = null;
    }

    public async void Show(AppleNativeNotification appleNativeNotification)
    {
        var current = UNUserNotificationCenter.Current;
        await current.RequestAlertAuthorization();

        using var content = new UNMutableNotificationContent();
        using var title = NSString.Create(appleNativeNotification.Title);
        content.Title = title;
        using var message = NSString.Create(appleNativeNotification.Message);
        content.Body = message;
        using var category = NSString.Create(appleNativeNotification.Category);
        content.CategoryIdentifier = category;

        using var id = NSString.Create(appleNativeNotification.AppleIdentifier);
        using var request = UNNotificationRequest.FromIdentifier(id, content);

        await UNUserNotificationCenter.Current.Add(request);

        if (appleNativeNotification.Expiration is { } expiration)
        {
            var closure = appleNativeNotification.AppleIdentifier;
            DispatcherTimer.RunOnce(() =>
            {
                UNUserNotificationCenter.Current.RemovePending([closure]);
            }, expiration);
        }
    }

    public void Close(AppleNativeNotification appleNativeNotification)
    {
        UNUserNotificationCenter.Current.RemovePending([appleNativeNotification.AppleIdentifier]);
    }
}
