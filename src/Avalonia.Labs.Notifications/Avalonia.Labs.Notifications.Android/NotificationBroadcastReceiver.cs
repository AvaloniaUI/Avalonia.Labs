using Android.Content;

namespace Avalonia.Labs.Notifications.Android
{
    [BroadcastReceiver(
        Exported = false,
        Enabled = true)]
    internal class NotificationBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent is { } i && i.Extras?.GetString("type") == "notification")
            {
                (Notifications.NativeNotificationManager.Current as NativeNotificationManager)?.OnReceivedIntent(i);
            }
        }
    }
}
