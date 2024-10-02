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

    public record NativeNotificationAction(string Caption, string Tag)
    {
        public Bitmap? Icon { get; init; }
    }

    public interface INativeNotificationManager
    {
        IReadOnlyDictionary<uint, INativeNotification> ActiveNotifications { get; }

        // if null, implementation will set a default category, otherwise category must be defined at launch
        INativeNotification? CreateNotification(string? category);
        void CloseAll();

        // triggered when an action is done, or the notification is cancelled
        event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;
    }

    internal interface INativeNotificationManagerImpl : INativeNotificationManager, IDisposable
    {
        void Initialize(AppNotificationOptions? options);
        NotificationChannelManager ChannelManager { get; }
    }

    public class NativeNotificationCompletedEventArgs : EventArgs
    {
        // tag of the action
        public string? ActionTag { get; init; }
        public uint? NotificationId { get; init; }

        // could be used for text input
        public object? UserData { get; init; }
        public bool IsCancelled { get; init; }
        public bool IsActivated { get; init; }
    }
}
