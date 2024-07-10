using System.Collections.Generic;

namespace Avalonia.Labs.Notifications
{
    public class NotificationChannelManager
    {
        public const string DefaultChannel = "default";
        public const string DefaultChannelLabel = "Notifications";

        private readonly Dictionary<string, NotificationChannel> _channels = new Dictionary<string, NotificationChannel>();

        public NotificationChannel AddChannel(NotificationChannel notificationChannel)
        {
            _channels[notificationChannel.Id] = notificationChannel;

            return notificationChannel;
        }

        public void DeleteChannel(string channel)
        {
            _channels.Remove(channel, out _);
        }

        public NotificationChannel? GetChannel(string id)
        {
            return _channels.GetValueOrDefault(id);
        }
    }
}
