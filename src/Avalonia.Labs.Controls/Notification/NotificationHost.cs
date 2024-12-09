using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls;


public partial class NotificationHost : TemplatedControl
{
    public NotificationHost()
    {
        _notificationsByPosition = new Dictionary<NotificationPosition, ObservableCollection<NotificationControl>>();
        foreach (var position in Enum.GetValues(typeof(NotificationPosition)).Cast<NotificationPosition>())
            _notificationsByPosition[position] = new ObservableCollection<NotificationControl>();

        // uses a dispatchertimer to check for expired notifications, but only as long as there are notifications present
        _notificationExpiryTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _notificationExpiryTimer.Tick += CheckExpiredNotifications;
    }
    
    /// <summary>
    /// Copied from Avalonia.Labs.Controls.DialogHost.
    /// Since we are put into the `OverlayLayer` we need calculate our desired size ourselves.
    /// We want to span across the whole screen always, so we return tl.ClientSize or VisualRoot.Bounds.Size
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        _ = base.MeasureOverride(availableSize);

        if (VisualRoot is TopLevel tl)
        {
            return tl.ClientSize;
        }
        else if (VisualRoot is Control c)
        {
            return c.Bounds.Size;
        }

        return default;
    }
   
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Bind collections to ItemsControls
        BindItemsControl(e, "PART_TopLeftNotifications", NotificationPosition.TopLeft);
        // BindItemsControl(e, "PART_TopCenterNotifications", NotificationPosition.TopCenter); // for now unavailable, since TopCenter was removed
        BindItemsControl(e, "PART_TopRightNotifications", NotificationPosition.TopRight);
        BindItemsControl(e, "PART_BottomLeftNotifications", NotificationPosition.BottomLeft);
        // BindItemsControl(e, "PART_BottomCenterNotifications", NotificationPosition.BottomCenter); // for now unavailable, since BottomCenter was removed
        BindItemsControl(e, "PART_BottomRightNotifications", NotificationPosition.BottomRight);
    }
    
    /// <summary>
    /// Shows a notification at the specified position to the user
    /// </summary>
    /// <param name="options"></param>
    /// <param name="position"></param>
    public void ShowNotification(NotificationOptions options, NotificationPosition position)
    {
        var notification = new NotificationControl(options);
        notification.Closed += OnNotificationClosed;

        var collection = _notificationsByPosition[position];
        
        if (IsTopPosition(position))
            collection.Insert(0, notification);
        else
            collection.Add(notification);

        // starts a single dispatchertimer (running on main thread with low prio) in case any notifications exist
        if (!_notificationExpiryTimer.IsEnabled)
            _notificationExpiryTimer.Start();

        notification.Open();
    }
    
    private bool IsTopPosition(NotificationPosition position) => position switch
    {
        NotificationPosition.TopLeft /*or NotificationPosition.TopCenter*/ or NotificationPosition.TopRight => true,
        _ => false
    };
    
    private void BindItemsControl(TemplateAppliedEventArgs e, string name, NotificationPosition position)
    {
        if (e.NameScope.Find<ItemsControl>(name) is ItemsControl itemsControl)
        {
            itemsControl.ItemsSource = _notificationsByPosition[position];
        }
    }

    private void OnNotificationClosed(object? sender, EventArgs e)
    {
        if (sender is NotificationControl notification)
        {
            // unsubscribe to avoid memory leak
            notification.Closed -= OnNotificationClosed;
            // remove the notification from its collection
            foreach (var collection in _notificationsByPosition.Values)
            {
                if (collection.Remove(notification))
                    break;
            }

            // if no more notification exists, we can stop the timer
            if (_notificationsByPosition.Values.All(c => c.Count == 0))
                _notificationExpiryTimer.Stop();
        }
    }

    private void CheckExpiredNotifications(object? sender, EventArgs e)
    {
        var now = DateTime.UtcNow;
        // check all notifications if they are expired and - thus - should be closed
        foreach (var notifications in _notificationsByPosition.Values)
        {
            var expiredNotifications = notifications
                .Where(n =>n.ShouldExpire(now))
                .ToList();

            foreach (var notification in expiredNotifications)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                // This is intentionally not awaited!!
                notification.Close(); 
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }
    }
    
    
    private readonly Dictionary<NotificationPosition, ObservableCollection<NotificationControl>> _notificationsByPosition;
    private readonly DispatcherTimer _notificationExpiryTimer;
    private bool _isMobileWidth;

}
