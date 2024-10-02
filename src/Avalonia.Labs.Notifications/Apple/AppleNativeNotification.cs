using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications.Apple;

[SupportedOSPlatform("ios")]
[SupportedOSPlatform("macos")]
internal class AppleNativeNotification : INativeNotification
{
    private readonly AppleNativeNotificationManager _manager;
    private static uint s_currentId = 0;

    public AppleNativeNotification(
        NotificationChannel channel, string bundleId,
        AppleNativeNotificationManager manager)
    {
        Id = Interlocked.Increment(ref s_currentId);
        AppleIdentifier = $"{bundleId}.notification.{Id}";
        Category = channel.Id;
        _manager = manager;
    }

    public uint Id { get; }
    public string AppleIdentifier { get; }
    public string Category { get; }

    public string? Title { get; set; }
    public string? Tag { get; set; }

    public string? Message { get; set; }

    public TimeSpan? Expiration { get; set; }
    public Bitmap? Icon { get; set; }
    public string? ReplyActionTag { get; set; }

    public IReadOnlyList<NativeNotificationAction> Actions { get; private set; }

    public void SetActions(IReadOnlyList<NativeNotificationAction> actions) => Actions = actions;

    public void Show()
    {
        _manager.Show(this);
    }

    public void Close()
    {
        _manager.Close(this);
    }
}
