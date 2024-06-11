using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications
{
    public interface INativeNotification
    {
        uint Id { get; }

        // categories are defined at launch. Defines which actions are set by default. on Android, will act as the notification channel
        string Category { get; }
        string? Title { get; set; }

        string? Tag { get; set; }
        string? Message { get; set; }
        TimeSpan? Expiration { get; set; }

        Bitmap? Icon { get; set; }

        // if set, enables text input in the notification and sets the specified action as the reply action
        string? ReplyActionTag { get; set; }

        // Defined by notification category
        IReadOnlyList<NativeNotificationAction> Actions { get; }

        // no-op on ios
        void SetActions(IReadOnlyList<NativeNotificationAction> actions);

        // can be called multiple times to update active notification
        void Show();
        void Close();
    }

    public class NativeNotificationAction
    {
        public string Tag { get; set; }
        public string Caption { get; set; }
        public Bitmap? Icon { get; set; }
    }

    public interface INativeNotificationManager
    {
        IDictionary<uint, INativeNotification> ActiveNotifications { get; }

        // if null, implementation will set a default category, otherwise category must be defined at launch
        INativeNotification? CreateNotification(string? category);
        void CloseAll();

        // triggered when an action is done, or the notification is cancelled
        event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;
    }

    public class NativeNotificationCompletedEventArgs : EventArgs
    {
        // tag of the action
        public string? ActionTag { get; set; }
        public uint? NotificationId { get; set; }

        // could be used for text input
        public object? UserData { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsActivated { get; set; }
    }
}
