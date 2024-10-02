using System.Collections.Generic;

namespace Avalonia.Labs.Notifications
{
    internal class NotificationChannelManager
    {
        public const string DefaultChannel = "default";
        public const string DefaultChannelLabel = "Notifications";

        protected readonly Dictionary<string, NotificationChannel> _channels = new Dictionary<string, NotificationChannel>();

        public virtual NotificationChannel AddChannel(NotificationChannel notificationChannel)
        {
            _channels[notificationChannel.Id] = notificationChannel;

            return notificationChannel;
        }

        public virtual void DeleteChannel(string channel)
        {
            _channels.Remove(channel, out _);
        }

        public NotificationChannel? GetChannel(string id)
        {
            return _channels.GetValueOrDefault(id);
        }
    }
}
