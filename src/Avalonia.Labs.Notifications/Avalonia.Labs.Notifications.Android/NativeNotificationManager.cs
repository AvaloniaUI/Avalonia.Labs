using Android;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace Avalonia.Labs.Notifications.Android
{
    internal class NativeNotificationManager : INativeNotificationManager
    {
        private readonly Dictionary<uint, INativeNotification> _notifications = new Dictionary<uint, INativeNotification>();
        private Activity _activity;
        private bool _isActive;

        public IDictionary<uint, INativeNotification> ActiveNotifications => _notifications;

        internal NotificationChannelManager ChannelManager { get; set; }

        public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

        public NativeNotificationManager(Activity activity)
        {
            _activity = activity;

            ChannelManager = new NotificationChannelManager(activity);

            if(_activity is IActivityIntentResultHandler handler)
                handler.OnActivityIntent += Activity_OnActivityIntent;
        }

        private void Activity_OnActivityIntent(object? sender, Intent e)
        {
            if (e.Extras?.GetString("type") == "notification")
            {
                (Notifications.NativeNotificationManager.Current as NativeNotificationManager)?.OnReceivedIntent(e);
            }
        }

        public void CloseAll()
        {
            foreach (var notification in _notifications)
            {
                notification.Value?.Close();
            }

            NotificationManagerCompat.From(_activity).CancelAll();

            _notifications.Clear();
        }

        public INativeNotification? CreateNotification(string? category)
        {
            if (!_isActive || _activity == null)
                return null;

            var channel = ChannelManager?.GetChannel(category ?? NotificationChannelManager.DefaultChannel) ??
                ChannelManager?.AddChannel(new NotificationChannel(NotificationChannelManager.DefaultChannel, NotificationChannelManager.DefaultChannelLabel));

            if (channel == null)
            {
                return null;
            }

            return new NativeNotification(_activity, this, channel);
        }

        public async void Initialize()
        {
            ChannelManager.ConsolidateChannels();
            _isActive = await CheckPermission();
        }

        private async Task<bool> CheckPermission()
        {
            if (_activity == null)
                return false;

            if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
                return true;

            return await PlatformSupport.CheckPermission(_activity, Manifest.Permission.PostNotifications);
        }

        internal async void Show(NativeNotification nativeNotification)
        {
            if (_activity != null && nativeNotification.CurrentNotification != null && await CheckPermission())
            {
                NotificationManagerCompat.From(_activity).Notify((int)nativeNotification.Id, nativeNotification.CurrentNotification);

                _notifications[nativeNotification.Id] = nativeNotification;
            }
        }

        internal void OnReceivedIntent(Intent? intent)
        {
            if (intent == null)
                return;

            var id = intent.Extras?.GetInt("notification-id");
            if (id != null)
            {
                var action = intent.Extras?.GetString("notification-action");
                var userAction = intent.Extras?.GetString("user-action");
                var eventArgs = new NativeNotificationCompletedEventArgs()
                {
                    NotificationId = (uint?)id,
                    IsActivated = action == "activate",
                    IsCancelled = action == "cancel",
                    ActionTag = userAction,
                    UserData = AndroidX.Core.App.RemoteInput.GetResultsFromIntent(intent)?.GetCharSequence(userAction)
                };

                NotificationCompleted?.Invoke(this, eventArgs);
            }
        }

        internal void Close(NativeNotification nativeNotification)
        {
            NotificationManagerCompat.From(_activity).Cancel((int)nativeNotification.Id);
            _notifications.Remove(nativeNotification.Id);
        }
    }
}
