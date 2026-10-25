#if BROWSER
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Avalonia.Labs.Notifications.Browser;

partial class BrowserNotificationManager : INativeNotificationManagerImpl
{
    Dictionary<uint, INativeNotification> currents = new();

    public NotificationChannelManager ChannelManager { get; private set; }
    public bool ClearOnClose { get; set; }
    public IReadOnlyDictionary<uint, INativeNotification> ActiveNotifications => currents;

    public BrowserNotificationManager()
    {
        ChannelManager = new NotificationChannelManager();
    }

    [JSImport("showNotification", "notifications")]
    public static partial Task ShowNotification(string title, string options);

    [JSImport("closeNotification", "notifications")]
    public static partial Task CloseNotification(string id);

    [JSImport("closeAllNotifications", "notifications")]
    public static partial Task CloseAllNotifications();

    [JSImport("registerServiceWorker", "notifications")]
    public static partial Task RegisterServiceWorker();

    [JSImport("isServiceWorkerSupported", "notifications")]
    public static partial bool IsServiceWorkerSupported();

    [JSImport("isSupported", "notifications")]
    public static partial bool IsSupported();

    [JSImport("registrations", "notifications")]
    public static partial Task RegisterHandlers(
        [JSMarshalAs<JSType.Function<JSType.String>>] Action<string> onclose,
        [JSMarshalAs<JSType.Function<JSType.String>>] Action<string> onclick,
        [JSMarshalAs<JSType.Function<JSType.String, JSType.String>>] Action<string, string> onreply);

    public async void OnClose(string data)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var obj = await JsonSerializer.DeserializeAsync(stream, NotificationJsonContext.Default.Data);
        if (obj != null && obj.data?.id is uint id && currents.TryGetValue(id, out var item))
        {
            NotificationCompleted?.Invoke(this, new NativeNotificationCompletedEventArgs()
            {
                IsCancelled = true,
                NotificationId = id
            });
        }
    }

    public async void OnClick(string data)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var obj = await JsonSerializer.DeserializeAsync(stream, NotificationJsonContext.Default.Data);
        if (obj != null && obj.data?.id is uint id && currents.TryGetValue(id, out var item))
        {
            NotificationCompleted?.Invoke(this, new NativeNotificationCompletedEventArgs()
            {
                ActionTag = obj.action,
                IsActivated = string.IsNullOrWhiteSpace(obj.action),
                IsCancelled = false,
                NotificationId = id
            });
        }
    }

    public async void OnReply(string data, string reply)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var obj = await JsonSerializer.DeserializeAsync(stream, NotificationJsonContext.Default.ReplyData);
        if (obj != null && obj.data?.id is uint id && currents.TryGetValue(id, out var item))
        {
            NotificationCompleted?.Invoke(this, new NativeNotificationCompletedEventArgs()
            {
                ActionTag = obj.action,
                IsActivated = string.IsNullOrWhiteSpace(obj.action),
                IsCancelled = false,
                NotificationId = id,
                UserData = reply
            });            
        }
    }

    public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

    public async void CloseAll()
    {
        await CloseAllNotifications();
        currents.Clear();
    }

    public INativeNotification? CreateNotification(string? category)
    {
        if (!IsSupported())
            return null;

        var channel = ChannelManager?.GetChannel(category ?? NotificationChannelManager.DefaultChannel) ??
            ChannelManager?.AddChannel(new NotificationChannel(NotificationChannelManager.DefaultChannel, NotificationChannelManager.DefaultChannelLabel));
        if (channel == null)
            return null;

        var item = new BrowserNotification(channel, this);
        currents.Add(item.Id, item);
        return item;
    }

    public async Task Show(BrowserNotification notification, NotificationOptions options)
    {
        await ShowNotification(notification.Title ?? string.Empty, JsonSerializer.Serialize(options, NotificationJsonContext.Default.NotificationOptions));
    }

    public async Task Close(uint id)
    {
        await CloseNotification(id.ToString());
        currents.Remove(id);
    }

    public void Dispose()
    {
        if (ClearOnClose)
            CloseAll();
    }

    public async void Initialize(AppNotificationOptions? options)
    {
        await JSHost.ImportAsync("notifications", "/_content/Avalonia.Labs.Notifications/notifications.js");
        if (IsServiceWorkerSupported())
        {
            await RegisterServiceWorker();
            await RegisterHandlers(OnClose, OnClick, OnReply);
        }
    }
}
#endif
