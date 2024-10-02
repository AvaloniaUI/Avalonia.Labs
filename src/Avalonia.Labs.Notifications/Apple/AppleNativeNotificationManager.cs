using System.Runtime.Versioning;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

[SupportedOSPlatform("ios")]
[SupportedOSPlatform("macos")]
internal class AppleNativeNotificationManager : INativeNotificationManager, IDisposable
{
    private readonly string _identifier;
    private UNUserNotificationCenterDelegate _notificationDelegate;

    public AppleNativeNotificationManager(string identifier)
    {
        _identifier = identifier;
    }

    public IDictionary<uint, INativeNotification> ActiveNotifications { get; }

    public INativeNotification? CreateNotification(string? category) => new AppleNativeNotification(category, _identifier, this);

    public void CloseAll() => UNUserNotificationCenter.Current?.RemoveAllPending();

    public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

    public void Initialize()
    {
        _notificationDelegate = new UNUserNotificationCenterDelegate();
        UNUserNotificationCenter.Current.Delegate = _notificationDelegate;
    }

    public void Dispose()
    {
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

        using var id = NSString.Create(appleNativeNotification.AppleIdentifier);
        using var request = UNNotificationRequest.FromIdentifier(id, content);

        await UNUserNotificationCenter.Current.Add(request);
    }

    public void Close(AppleNativeNotification appleNativeNotification)
    {
        UNUserNotificationCenter.Current.RemovePending([appleNativeNotification.Id.ToString()]);
    }
}
