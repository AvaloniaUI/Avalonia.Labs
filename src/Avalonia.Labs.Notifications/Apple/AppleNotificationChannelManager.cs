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
            if (CreateCategory(channel.Value) is { } category)
                categories.Add(category);
        }

        return NSSet.WithObjects(categories);
    }

    private static UNNotificationCategory? CreateCategory(NotificationChannel channel)
    {
        var actions = new List<UNNotificationAction>();
        foreach (var action in channel.Actions)
        {
            var nativeAction = action.Tag == "reply" // TODO: hardcoded
                ? UNNotificationAction.CreateTextInput(action.Tag, action.Caption, "Reply", null)
                : UNNotificationAction.Create(action.Tag, action.Caption);
            if (nativeAction is not null)
                actions.Add(nativeAction);
        }

        return UNNotificationCategory.Create(channel.Id, actions);
    }
}
