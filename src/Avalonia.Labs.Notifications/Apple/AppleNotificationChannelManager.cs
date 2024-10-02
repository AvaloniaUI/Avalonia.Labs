using System.Collections.Generic;

namespace Avalonia.Labs.Notifications.Apple;

internal class AppleNotificationChannelManager : NotificationChannelManager
{
    public void RegisterTo(UNUserNotificationCenter center)
    {
        var categories = new List<UNNotificationCategory>();
        try
        {
            foreach (var channel in _channels)
            {
                categories.Add(CreateCategory(channel.Value));
            }

            center.SetNotificationCategories(categories);
        }
        finally
        {
            foreach (var category in categories)
            {
                category.Dispose();
            }
        }
    }

    private static UNNotificationCategory CreateCategory(NotificationChannel channel)
    {
        var actions = new List<UNNotificationAction>();
        try
        {
            foreach (var action in channel.Actions)
            {
                actions.Add(UNNotificationAction.Create(action.Tag, action.Caption));
            }

            return UNNotificationCategory.Create(channel.Id, actions);
        }
        finally
        {
            foreach (var action in actions)
            {
                action.Dispose();
            }
        }
    }
}
