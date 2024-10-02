using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using AppleInterop;
using Avalonia.Threading;

namespace Avalonia.Labs.Notifications.Apple;

[SupportedOSPlatform("ios")]
[SupportedOSPlatform("macos")]
internal class AppleNativeNotificationManager : INativeNotificationManagerImpl, IDisposable
{
    private readonly string _identifier;
    private readonly UNUserNotificationCenterDelegate _notificationDelegate;
    private readonly Dictionary<string, (INativeNotification, UNNotificationRequest)> _notifications = [];

    public AppleNativeNotificationManager(string identifier)
    {
        _identifier = identifier;
        _notificationDelegate = new UNUserNotificationCenterDelegate();
        ChannelManager = new AppleNotificationChannelManager();
    }

    public AppleNotificationChannelManager ChannelManager { get; }
    NotificationChannelManager INativeNotificationManagerImpl.ChannelManager => ChannelManager;

    public IReadOnlyDictionary<uint, INativeNotification> ActiveNotifications =>
        _notifications.ToDictionary(
            n => n.Value.Item1.Id,
            n => n.Value.Item1);

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

    public void Initialize(AppNotificationOptions? options)
    {
        var current = UNUserNotificationCenter.Current;
        _notificationDelegate.DidReceiveNotificationResponse += NotificationDelegateOnDidReceiveNotificationResponse;
        current.Delegate = _notificationDelegate;

        current.SetNotificationCategories(ChannelManager.ToSet());
    }

    private void NotificationDelegateOnDidReceiveNotificationResponse(object? sender, (string notificationId, string actionId, string? userText) e)
    {
        if (!_notifications.TryGetValue(e.notificationId, out var notificationTuple))
            return;

        const string defaultActionId = "com.apple.UNNotificationDefaultActionIdentifier";
        NotificationCompleted?.Invoke(this, new NativeNotificationCompletedEventArgs
        {
            NotificationId = notificationTuple.Item1.Id,
            IsActivated = e.actionId == defaultActionId,
            ActionTag = e.actionId == defaultActionId ? null : e.actionId,
            UserData = e.userText
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

        var content = new UNMutableNotificationContent();
        content.Title = CFString.Create(appleNativeNotification.Title);
        content.Body = CFString.Create(appleNativeNotification.Message);
        content.CategoryIdentifier = CFString.Create(appleNativeNotification.Category);

        var request = UNNotificationRequest.FromIdentifier(
            CFString.Create(appleNativeNotification.AppleIdentifier), content);
        _notifications[appleNativeNotification.AppleIdentifier] = (appleNativeNotification, request);

        _ = UNUserNotificationCenter.Current.Add(request);

        if (appleNativeNotification.Expiration is { } expiration)
        {
            var closure = appleNativeNotification.AppleIdentifier;
            DispatcherTimer.RunOnce(() =>
            {
                UNUserNotificationCenter.Current.RemovePending(closure);
            }, expiration);
        }
    }

    public void Close(AppleNativeNotification appleNativeNotification)
    {
        UNUserNotificationCenter.Current.RemovePending(appleNativeNotification.AppleIdentifier);
    }
}
