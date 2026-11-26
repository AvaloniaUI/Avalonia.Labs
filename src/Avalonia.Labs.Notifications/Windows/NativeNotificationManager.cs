#if INCLUDE_WINDOWS
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.Logging;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications.Windows
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal class NativeNotificationManager : INativeNotificationManagerImpl, IDisposable
    {
        public const string NotificationsGroupName = "AvaloniaUI";
        private const string LogArea = "Notifications";
        private readonly Dictionary<uint, INativeNotification> _notifications = new Dictionary<uint, INativeNotification>();
        private (string id, bool syntetic) _aumid;
        private Guid? _serverUuid;

        public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

        public IReadOnlyDictionary<uint, INativeNotification> ActiveNotifications => _notifications;

        public NotificationChannelManager ChannelManager { get; }
        public bool ClearOnClose { get; set; }
        public NativeNotificationManager()
        {
            ChannelManager = new NotificationChannelManager();
        }

        public void CloseAll()
        {
            var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");
            if (managerStatics is IToastNotificationManagerStatics2 manager2)
            {
                if (_aumid.syntetic)
                {
                    using var hstring = new HStringWrapper(_aumid.id);
                    manager2.GetHistory().ClearWithId(hstring);
                }
                else
                {
                    manager2.GetHistory().Clear();
                }
            }
        }

        public void Initialize(AppNotificationOptions? options)
        {
            _aumid = !DesktopBridgeHelpers.HasPackage() && options?.AppUserModelId is not null ? (options.AppUserModelId, true) : AumidHelper.GetAumid();

            if (options is null || !options.DisableComServer)
            {
                var serverUuid = options?.ComActivatorGuidOverride ?? AumidHelper.GetGuidFromId(_aumid.id);
                try
                {
                    NotificationComServer.CreateAndRegisterActivator(serverUuid);
                    _serverUuid = serverUuid;
                }
                catch (Exception ex)
                {
                    // E.g. locked-down COM security policies. Toasts still work, only activations are lost.
                    Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(this,
                        "Unable to register the notification COM activator {Guid}. NotificationCompleted won't be raised for toast activations. {Exception}",
                        serverUuid, ex);
                }
            }

            if (_aumid.syntetic)
            {
                try
                {
                    AumidHelper.RegisterAumid(_aumid.id, _serverUuid, options?.AppName, options?.AppIcon);
                }
                catch (Exception ex)
                {
                    Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(this,
                        "Unable to register AppUserModelId {Aumid} for notifications. {Exception}",
                        _aumid.id, ex);
                }
            }
        }

        public INativeNotification? CreateNotification(string? category)
        {
            var channel = ChannelManager?.GetChannel(category ?? NotificationChannelManager.DefaultChannel) ??
                ChannelManager?.AddChannel(new NotificationChannel(NotificationChannelManager.DefaultChannel, NotificationChannelManager.DefaultChannelLabel));

            if (channel == null)
            {
                return null;
            }
            return new NativeNotification(channel, this);
        }

        internal void OnNotificationReceived(string invokedArgs, Dictionary<string, string> pairs, string appUserModelId)
        {
            if (appUserModelId != _aumid.id)
                return;
            Dictionary<string, string> args = new Dictionary<string, string>();
            var splits = invokedArgs.Split(';');
            foreach(var split in splits)
            {
                var parts = split.Split('=', 2);
                if (parts.Length == 2)
                    args[parts[0]] = Uri.UnescapeDataString(parts[1]);
            }

            if (args.TryGetValue("notificationId", out var notificationIdString)
                && uint.TryParse(notificationIdString, NumberStyles.None, CultureInfo.InvariantCulture, out var notificationId))
            {
                var action = args.GetValueOrDefault("action");
                var userAction = action == "user" ? args.GetValueOrDefault("userAction") : null;
                var input = pairs.GetValueOrDefault("input");
                var eventArgs = new NativeNotificationCompletedEventArgs()
                {
                    NotificationId = notificationId,
                    NotificationTag = args.GetValueOrDefault("tag"),
                    IsActivated = action == "activate",
                    ActionTag = userAction,
                    UserData = input
                };

                NotificationCompleted?.Invoke(this, eventArgs);
            }
        }

        internal void Show(NativeNotification nativeNotification)
        {
            if (nativeNotification.CurrentNotification == null)
                return;

            var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");

            IToastNotifier notifier;

            if (_aumid.syntetic)
            {
                using var appIdHRef = new HStringWrapper(_aumid.id);
                notifier = managerStatics.CreateToastNotifierWithId(appIdHRef);
            }
            else
            {
                notifier = managerStatics.CreateToastNotifier();
            }

            notifier.Show(nativeNotification.CurrentNotification);
            _notifications[nativeNotification.Id] = nativeNotification;
        }

        internal void Close(NativeNotification nativeNotification)
        {
            var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");
            if(managerStatics is IToastNotificationManagerStatics2 manager2)
            {
                using var tagStr = new HStringWrapper(nativeNotification.Id.ToString());
                using var groupStr = new HStringWrapper(NotificationsGroupName);

                if (_aumid.syntetic)
                {
                    using var appIdStr = new HStringWrapper(_aumid.id);
                    manager2.GetHistory().RemoveGroupedTagWithId(tagStr, groupStr, appIdStr);
                }
                else
                {
                    manager2.GetHistory().RemoveGroupedTag(tagStr, groupStr);
                }
            }
            _notifications.Remove(nativeNotification.Id);
        }

        public void Dispose()
        {
            var path = GetAppDataFolderPath();
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            _notifications.Clear();

            if (ClearOnClose)
            {
                CloseAll();
            }

            if (_serverUuid is not null)
            {
                NotificationComServer.UnregisterActivator();
                NotificationComServer.DeleteActivatorRegistration(_serverUuid.Value);
            }
        }

        private string GetAppDataFolderPath()
        {
            return _aumid.syntetic ?
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ToastNotificationManagerCompat", "Apps", _aumid.id.Split("!")[0]) :
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Packages", _aumid.id.Split("!")[0], "Avalonia.Labs.Notifications");
        }

        internal string? SaveBitmapToAppPath(Bitmap bitmap)
        {
            try
            {
                var folder = GetAppDataFolderPath();
                var name = Convert.ToHexString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString() + Random.Shared.Next()))) + ".png";
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, name);
                bitmap.Save(path, PngBitmapEncoderOptions.Default);

                return new Uri(path).AbsoluteUri;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
#endif
