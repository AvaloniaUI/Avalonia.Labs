using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly Dictionary<uint, INativeNotification> _notifications = [];

    public AppleNativeNotificationManager(string identifier)
    {
        _identifier = identifier;
        _notificationDelegate = new UNUserNotificationCenterDelegate();
        ChannelManager = new AppleNotificationChannelManager();
    }

    public AppleNotificationChannelManager ChannelManager { get; }
    public IDictionary<uint, INativeNotification> ActiveNotifications => _notifications;

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

    public void CloseAll()
    {
        UNUserNotificationCenter.Current?.RemoveAllPending();
        _notifications.Clear();
    }

    public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

    public void Initialize()
    {
        var current = UNUserNotificationCenter.Current;
        _notificationDelegate.DidReceiveNotificationResponse += NotificationDelegateOnDidReceiveNotificationResponse;
        current.Delegate = _notificationDelegate;
        ChannelManager.RegisterTo(current);
    }

    private void NotificationDelegateOnDidReceiveNotificationResponse(object? sender, (string notificationId, string actionId) e)
    {
        var notificationId = uint.Parse(e.notificationId.Split('.').Last());
        if (!_notifications.TryGetValue(notificationId, out var notification))
            return;

        const string defaultActionId = "com.apple.UNNotificationDefaultActionIdentifier";
        NotificationCompleted?.Invoke(this, new NativeNotificationCompletedEventArgs
        {
            NotificationId = notification.Id,
            IsActivated = e.actionId == defaultActionId,
            ActionTag = e.actionId == defaultActionId ? null : e.actionId
        });
    }

    public void Dispose()
    {
        _notificationDelegate.DidReceiveNotificationResponse -= NotificationDelegateOnDidReceiveNotificationResponse;
        UNUserNotificationCenter.Current.Delegate = null;
    }

    public async void Show(AppleNativeNotification appleNativeNotification)
    {
        var current = UNUserNotificationCenter.Current;
        var result = await current.RequestAlertAuthorization();
        if (!result)
            return;

        using var content = new UNMutableNotificationContent();
        using var title = NSString.Create(appleNativeNotification.Title);
        content.Title = title;
        using var message = NSString.Create(appleNativeNotification.Message);
        content.Body = message;
        using var category = NSString.Create(appleNativeNotification.Category);
        content.CategoryIdentifier = category;

        using var id = NSString.Create(appleNativeNotification.AppleIdentifier);
        using var request = UNNotificationRequest.FromIdentifier(id, content);

        _notifications[appleNativeNotification.Id] = appleNativeNotification;
        _ = UNUserNotificationCenter.Current.Add(request);

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
        _notifications.Remove(appleNativeNotification.Id);
        UNUserNotificationCenter.Current.RemovePending([appleNativeNotification.AppleIdentifier]);
    }
}
