#if ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Android.App;
using Android.OS;
using AndroidX.Core.App;

namespace Avalonia.Labs.Notifications.Android
{
    internal class AndroidNotificationChannelManager : NotificationChannelManager
    {
        private readonly global::Android.Content.Context _context;

        internal static bool SupportsChannels => Build.VERSION.SdkInt >= BuildVersionCodes.O;


        public AndroidNotificationChannelManager(global::Android.Content.Context context)
        {
            _context = context;
        }

        public override NotificationChannel AddChannel(NotificationChannel notificationChannel)
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
                    _ => throw new NotImplementedException(),
                }));
                NotificationManagerCompat.From(_context).CreateNotificationChannel(builder.Build());
            }

            _channels[notificationChannel.Id] = notificationChannel;

            return notificationChannel;
        }

        [SupportedOSPlatform("android26.0")]
        private string[] GetAppChannels()
        {
            if (!SupportsChannels)
                return Array.Empty<string>();
            var manager = NotificationManagerCompat.From(_context);
            var channels = manager.NotificationChannels;

            return channels.Select(c => c.Id ?? "").ToArray();
        }

        [SupportedOSPlatform("android26.0")]
        public override void DeleteChannel(string channel)
        {
            if(_channels.TryGetValue(channel, out _))
            {
                _channels.Remove(channel);
            }

            if (SupportsChannels && GetAppChannels().Contains(channel))
            {
                NotificationManagerCompat.From(_context).DeleteNotificationChannel(channel);
            }
        }

        public override NotificationChannel? GetChannel(string id)
        {
            if (_channels.TryGetValue(id, out var channel))
                return channel;

            return null;
        }

        [SupportedOSPlatform("android26.0")]
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
#endif
