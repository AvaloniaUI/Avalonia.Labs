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
            if (_channels.TryGetValue(channel, out _))
            {
                _channels.Remove(channel);
            }
        }

        public NotificationChannel? GetChannel(string id)
        {
            if (_channels.TryGetValue(id, out var channel))
                return channel;

            return null;
        }
    }
}
