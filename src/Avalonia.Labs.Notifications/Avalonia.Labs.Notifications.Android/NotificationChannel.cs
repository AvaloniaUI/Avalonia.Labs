using Android.OS;
using AndroidX.Core.App;

namespace Avalonia.Labs.Notifications.Android
{
    internal class NotificationChannelManager
    {
        public const string DefaultChannel = "default";
        public const string DefaultChannelLabel = "Notifications";
        private readonly Activity _activity;

        private readonly Dictionary<string, NotificationChannel> _channels = new Dictionary<string, NotificationChannel>();

        internal static bool SupportsChannels => Build.VERSION.SdkInt >= BuildVersionCodes.O;


        public NotificationChannelManager(Activity activity)
        {
            _activity = activity;
        }

        public NotificationChannel AddChannel(NotificationChannel notificationChannel)
        {
            if (SupportsChannels)
            {
                var builder = new NotificationChannelCompat.Builder(notificationChannel.Id, NotificationManagerCompat.ImportanceDefault);
                builder.SetName(notificationChannel.Label);
                builder.SetImportance((int)(notificationChannel.Priority switch
                {
                    NotificationPriority.Default => NotificationImportance.Default,
                    NotificationPriority.Low => NotificationImportance.Low,
                    NotificationPriority.High => NotificationImportance.High,
                    NotificationPriority.Max => NotificationImportance.Max,
                }));
                NotificationManagerCompat.From(_activity).CreateNotificationChannel(builder.Build());
            }

            _channels[notificationChannel.Id] = notificationChannel;

            return notificationChannel;
        }

        private string[] GetAppChannels()
        {
            if (!SupportsChannels)
                return Array.Empty<string>();
            var manager = NotificationManagerCompat.From(_activity);
            var channels = manager.NotificationChannels;

            return channels.Select(c => c.Id ?? "").ToArray();
        }

        public void DeleteChannel(string channel)
        {
            if(_channels.TryGetValue(channel, out _))
            {
                _channels.Remove(channel);
            }

            if (SupportsChannels && GetAppChannels().Contains(channel))
            {
                NotificationManagerCompat.From(_activity).DeleteNotificationChannel(channel);
            }
        }

        public NotificationChannel? GetChannel(string id)
        {
            if (_channels.TryGetValue(id, out var channel))
                return channel;

            return null;
        }

        internal void ConsolidateChannels()
        {
            var unknownChannels = GetAppChannels().Where(x => !_channels.ContainsKey(x));

            foreach(var channel in unknownChannels)
            {
                DeleteChannel(channel);
            }
        }
    }
}
