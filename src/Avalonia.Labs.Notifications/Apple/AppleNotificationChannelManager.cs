using System.Collections.Generic;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class AppleNotificationChannelManager : NotificationChannelManager
{
    public NSSet ToSet()
    {
        var categories = new List<UNNotificationCategory>();
        foreach (var channel in _channels)
        {
            categories.Add(CreateCategory(channel.Value));
        }

        return NSSet.WithObjects(categories);
    }

    private static UNNotificationCategory? CreateCategory(NotificationChannel channel)
    {
        var actions = new List<UNNotificationAction>();
        foreach (var action in channel.Actions)
        {
            actions.Add(action.Tag == "reply" // TODO: hardcoded
                ? UNNotificationAction.CreateTextInput(action.Tag, action.Caption, "Reply", null)
                : UNNotificationAction.Create(action.Tag, action.Caption));
        }

        return UNNotificationCategory.Create(channel.Id, actions);
    }
}
