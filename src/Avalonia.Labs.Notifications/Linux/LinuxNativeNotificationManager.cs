using static Avalonia.Labs.Notifications.Linux.NativeInterop;

namespace Avalonia.Labs.Notifications.Linux
{
    internal class LinuxNativeNotificationManager : INativeNotificationManager, IDisposable
    {
        public const string DefaultAction = "default";
        public delegate void ActionDelegate(IntPtr notification, string action, IntPtr user_data);
        private readonly Dictionary<uint, INativeNotification> _notifications = new Dictionary<uint, INativeNotification>();
        private string _appName;
        private readonly string? _appIcon;
        private bool _isInitialized;
        private Thread? _loopThread;
        private IntPtr _loop;
        public ActionDelegate Callback = new ActionDelegate(Activated);

        public LinuxNativeNotificationManager(string appName, string? appIcon)
        {
            _appName = appName;
            _appIcon = appIcon;
            ChannelManager = new NotificationChannelManager();
        }

        public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

        public IDictionary<uint, INativeNotification> ActiveNotifications => _notifications;

        internal NotificationChannelManager ChannelManager { get; set; }

        public string? AppIcon => _appIcon;

        public string AppName { get => _appName; set => _appName = value; }

        public INativeNotification? CreateNotification(string? category)
        {
            if (string.IsNullOrWhiteSpace(_appName) || !_isInitialized)
            {
                return null;
            }
            var channel = ChannelManager?.GetChannel(category ?? NotificationChannelManager.DefaultChannel) ??
                ChannelManager?.AddChannel(new NotificationChannel(NotificationChannelManager.DefaultChannel, NotificationChannelManager.DefaultChannelLabel));

            if (channel == null)
            {
                return null;
            }
            return new LinuxNativeNotification(channel, this);
        }

        internal unsafe void Initialize()
        {
            if (string.IsNullOrWhiteSpace(_appName) || _isInitialized)
            {
                return;
            }

            try
            {
                _isInitialized = notify_init(_appName);

                if (_isInitialized && _loopThread == null)
                {
                    _loopThread = new Thread(() =>
                    {
                        _loop = g_main_loop_new(IntPtr.Zero, false);
                        g_main_loop_run(_loop);
                    });

                    _loopThread.Start();
                }
            }
            catch (Exception _)
            {
                _isInitialized = false;
            }
        }

        internal void Show(LinuxNativeNotification notification)
        {
            if (!_isInitialized)
            {
                return;
            }

            if (notification != null)
            {
                IntPtr error = default;
                var successfull = notify_notification_show(notification.NativeHandle, ref error);

                if (!successfull)
                {
                    CloseNotification(notification);

                    g_error_free(error);
                }
                else
                {
                    _notifications.TryAdd(notification.Id, notification);
                }
                notification.DeleteNativeResources();
            }
        }

        public void CloseAll()
        {
            foreach (var notification in _notifications)
            {
                CloseNotification(notification.Value as LinuxNativeNotification);
            }

            _notifications.Clear();
        }


        internal static void CloseNotification(LinuxNativeNotification? notification)
        {
            if (notification != null && notification.NativeHandle != default)
            {
                IntPtr error = default;
                notify_notification_close(notification.NativeHandle, ref error);

                if (error != default)
                    g_error_free(error);

                notification.NativeHandle = default;
            }
        }


        private static void Activated(IntPtr notification, string action, IntPtr user_data)
        {
            var id = (uint)user_data;

            if (NativeNotificationManager.Current is LinuxNativeNotificationManager manager)
            {
                if (manager._notifications.ContainsKey(id) && manager._notifications.Remove(id, out var nativeNotification))
                {
                    var eventArgs = new NativeNotificationCompletedEventArgs()
                    {
                        NotificationId = id,
                        IsActivated = action == DefaultAction,
                        ActionTag = action == DefaultAction ? null : action,
                    };

                    manager.NotificationCompleted?.Invoke(manager, eventArgs);

                    manager.Close(nativeNotification as LinuxNativeNotification);
                }
            }
        }

        public void Dispose()
        {
            if (_loop != default)
                g_main_loop_quit(_loop);

            CloseAll();

            _loop = default;
            notify_uninit();
        }

        internal void Close(LinuxNativeNotification? linuxNativeNotification)
        {
            if (linuxNativeNotification != null)
            {
                CloseNotification(linuxNativeNotification);
                _notifications.Remove(linuxNativeNotification.Id);
            }
        }
    }
}
