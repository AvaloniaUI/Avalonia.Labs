using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls;

public class NotificationManager : INotificationManager
{
    private readonly Dictionary<TopLevel, Controls.NotificationHost> _hosts = new();
    
    private NotificationManager()
    {}

    public static INotificationManager Default = new NotificationManager();

    public NotificationPosition DefaultNotificationPosition { get; set; } = NotificationPosition.TopLeft;
    public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromSeconds(3);
    
    public void ShowNotification(NotificationOptions options)
    {
        if (options.Duration == null && options.IsExpired == null)
        {
            options.Duration = DefaultDuration;
        }

        if (options.Position is null)
        {
            options.Position = DefaultNotificationPosition;
        }

        Dispatcher.UIThread.Post(() =>
        {
            var topLevel = Utilities.GetTopLevel(null);
            var host = GetOrCreateHost(topLevel!);
            host.ShowNotification(options, options.Position.Value);
        });
        
        
    }

    private Controls.NotificationHost GetOrCreateHost(TopLevel topLevel)
    {
        if (topLevel == null) throw new ArgumentNullException(nameof(topLevel));
        
        var overlayLayer = OverlayLayer.GetOverlayLayer(topLevel);
        var host = overlayLayer!.Children.OfType<Controls.NotificationHost>().FirstOrDefault();
        if (host == null)
        {
            host = new Controls.NotificationHost();
            // ensure that the notificationhost is always below the dialoghost - since modal dialgs should also block notifications
            overlayLayer.Children.Insert(0, host);
        }
        return host;
    }
}
