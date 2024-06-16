namespace Avalonia.Labs.Notifications
{
    public class NotificationChannel
    {
        public List<NativeNotificationAction> Actions { get; set; }

        public string Id { get; }

        public string Label { get; }
        public NotificationPriority Priority { get; }

        public NotificationChannel(string id, string label, NotificationPriority priority = NotificationPriority.Default)
        {
            Id = id;
            Label = label;

            Actions = new List<NativeNotificationAction>();
            Priority = priority;
        }
    }

    public enum NotificationPriority
    {
        Default,
        Low,
        High,
        Max
    }
}
