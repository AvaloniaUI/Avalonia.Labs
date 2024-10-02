#if ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications.Android
{
    internal class NativeNotification : INativeNotification
    {
        private static uint s_currentId = 0;
        private readonly Activity _activity;
        private readonly NativeNotificationManager _manager;
        private readonly NotificationChannel _channel;

        public uint Id { get; }

        public string Category => _channel.Id;

        public string? Title { get; set; }

        public string? Tag { get; set; }
        public string? Message { get; set; }
        public TimeSpan? Expiration { get; set; }

        public Bitmap? Icon { get; set; }

        public IReadOnlyList<NativeNotificationAction> Actions { get; private set; }
        public Notification? CurrentNotification { get; private set; }
        public string? ReplyActionTag { get; set; }

        public NativeNotification(Activity activity, NativeNotificationManager manager, NotificationChannel channel)
        {
            _activity = activity;
            _manager = manager;
            _channel = channel;

            Id = GetNextId();

            Actions = channel.Actions;
        }

        public void Close()
        {
            _manager.Close(this);
        }

        public void SetActions(IReadOnlyList<NativeNotificationAction> actions)
        {
            Actions = actions;
        }

        public void Show()
        {
            var builder = new NotificationCompat.Builder(_activity, _channel.Id);

            if (_activity.ApplicationInfo?.Icon is { } iconResource)
            {
                builder.SetSmallIcon(iconResource);
            }
            if (Icon != null)
            {
                var androidBitmap = Icon.ToAndroid();
                if (androidBitmap != null)
                    builder.SetLargeIcon(androidBitmap);
            }

            builder.SetContentTitle(Title);
            builder.SetContentText(Message);
            builder.SetPriority(NotificationCompat.PriorityDefault);
            builder.SetAutoCancel(true);

            var pendingIntentId = string.IsNullOrEmpty(Tag) ? GetHashCode() + Random.Shared.Next() : Tag.GetHashCode();

            //set notification intent
            var tapBundle = new Bundle();
            var deleteBundle = new Bundle();
            tapBundle.PutString("type", "notification");
            tapBundle.PutString("notification-id", Id.ToString());
            tapBundle.PutString("notification-action", "activate");
            deleteBundle.PutString("type", "notification");
            deleteBundle.PutString("notification-id", Id.ToString());
            deleteBundle.PutString("notification-action", "cancel");
            var tapIntent = new Intent(_activity, _activity.Class)
                .SetFlags(ActivityFlags.SingleTop)
                .PutExtras(tapBundle);

            var deleteIntent = new Intent(_activity, typeof(NotificationBroadcastReceiver))
                .SetFlags(ActivityFlags.SingleTop)
                .PutExtras(deleteBundle);

            var flags = Build.VERSION.SdkInt >= BuildVersionCodes.S ? PendingIntentFlags.Mutable : PendingIntentFlags.UpdateCurrent;

            builder.SetContentIntent(PendingIntent.GetActivity(_activity, pendingIntentId + 1, tapIntent, flags))
                .SetDeleteIntent(PendingIntent.GetBroadcast(_activity, pendingIntentId + 2, deleteIntent, flags));

            if(!AndroidNotificationChannelManager.SupportsChannels)
            {
                builder.SetPriority(_channel.Priority switch
                {
                    NotificationPriority.Default => NotificationCompat.PriorityDefault,
                    NotificationPriority.Low => NotificationCompat.PriorityLow,
                    NotificationPriority.High => NotificationCompat.PriorityHigh,
                    NotificationPriority.Max => NotificationCompat.PriorityMax,
                    _ => throw new NotImplementedException(),
                });
            }

            var replyAction = Actions.FirstOrDefault(x => x.Tag == ReplyActionTag);

            for (var i = 0; i < Actions.Count; i++)
            {
                var action = Actions[i];
                if (string.IsNullOrWhiteSpace(action.Tag))
                    continue;

                var actionbundle = new Bundle();
                actionbundle.PutString("type", "notification");
                actionbundle.PutString("notification-id", Id.ToString());
                tapBundle.PutString("notification-action", "action");
                actionbundle.PutString("user-action", action.Tag);

                var icon = action.Icon != null && action.Icon.ToAndroid() is { } bitmap ? IconCompat.CreateWithBitmap(bitmap) : IconCompat.CreateWithResource(_activity, _activity.ApplicationInfo?.Icon ?? 0);

                var actionIntent = new Intent(_activity, typeof(NotificationBroadcastReceiver))
                    .PutExtras(actionbundle);
                var pendingIntent = PendingIntent.GetBroadcast(_activity, pendingIntentId + 3 + i, actionIntent, replyAction != null ? PendingIntentFlags.UpdateCurrent : flags);
                var notificationAction = new NotificationCompat.Action.Builder(icon, action.Caption, pendingIntent);

                if (!string.IsNullOrWhiteSpace(ReplyActionTag) && replyAction == action)
                {
                    var remoteInputBuilder = new AndroidX.Core.App.RemoteInput.Builder(ReplyActionTag);
                    remoteInputBuilder.SetLabel(action.Caption);
                    notificationAction.AddRemoteInput(remoteInputBuilder.Build());
                }


                builder.AddAction(notificationAction.Build());
            }

            CurrentNotification = builder.Build();

            _manager.Show(this);
        }

        private uint GetNextId()
        {
            return Interlocked.Increment(ref s_currentId);
        }
    }
}
#endif
