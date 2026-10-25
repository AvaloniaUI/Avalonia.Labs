#if BROWSER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications.Browser;

class BrowserNotification : INativeNotification
{
    private static uint s_currentId = 0;
    BrowserNotificationManager manager;
    string category;

    public BrowserNotification(NotificationChannel channel, BrowserNotificationManager manager)
    {
        this.category = channel.Id;
        this.manager = manager;
        this.Actions = channel.Actions;
        this.ChannelIcon = channel.Icon;
        this.Vibrations = channel.Vibrations ?? [];

        Id = GetNextId();
    }

    public uint Id { get; }
    public string? ReplyActionTag {  get; set; }
    public int[] Vibrations { get; set; }
    public string Category => category;
    public string? ChannelIcon { get; set; }
    public string? Title { get; set; }
    public string? Tag { get; set; }
    public string? Message { get; set; }
    public TimeSpan? Expiration { get; set; }
    public IReadOnlyList<NativeNotificationAction> Actions { get; private set; }
    public Bitmap? Icon { get; set; }

    public async void Close()
    {
        await manager.Close(Id);
    }

    public async void Show()
    {
        string? icon = null;
        if (ChannelIcon == null && Icon != null)
        {
            using var memStream = new MemoryStream();// Icon.PixelSize.Width * Icon.PixelSize.Height * 4);
            Icon.Save(memStream);
            icon = $"data:image/png;base64,{Convert.ToBase64String(memStream.ToArray())}";
        }

        await manager.Show(this, new NotificationOptions()
        {
            Actions = Actions?.Select(a => new NotificationAction { Action = a.Tag, Icon = a.ActionIcon, Title = a.Caption, Type = a.Tag == ReplyActionTag ? "text" : "button" }).ToArray() ?? [],
            Body = Message,
            Data = new NotificationData
            {
                Id = Id,
                ReplyActionTag = ReplyActionTag,
            },
            Icon = ChannelIcon ?? icon,
            Tag = Tag,
            Vibrations = Vibrations
        });
    }

    private static uint GetNextId()
    {
        return Interlocked.Increment(ref s_currentId);
    }

    public void SetActions(IReadOnlyList<NativeNotificationAction> actions)
    {
        Actions = actions;
    }
}
#endif
