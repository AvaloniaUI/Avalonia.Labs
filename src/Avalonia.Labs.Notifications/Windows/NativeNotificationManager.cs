#if INCLUDE_WINDOWS
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.Media.Imaging;
using MicroCom.Runtime;

namespace Avalonia.Labs.Notifications.Windows
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal class NativeNotificationManager : INativeNotificationManagerImpl, IDisposable
    {
        public const string NotificationsGroupName = "AvaloniaUI"; 
        private readonly Dictionary<uint, INativeNotification> _notifications = new Dictionary<uint, INativeNotification>();
        private (string id, bool syntetic) _aumid;
        private Guid? _serverUuid;

        public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

        public IReadOnlyDictionary<uint, INativeNotification> ActiveNotifications => _notifications;

        public NotificationChannelManager ChannelManager { get; }
        public NativeNotificationManager()
        {
            ChannelManager = new NotificationChannelManager();
        }

        public void CloseAll()
        {
            using var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");
            if (managerStatics.QueryInterface<IToastNotificationManagerStatics2>() is { } manager2)
            {
                if (_aumid.syntetic)
                {
                    using var hstring = new HStringWrapper(_aumid.id);
                    manager2.History.ClearWithId(hstring);
                }
                else
                {
                    manager2.History.Clear();
                }
            }
        }

        public void Initialize(AppNotificationOptions? options)
        {
            _aumid = !DesktopBridgeHelpers.HasPackage() && options?.AppUserModelId is not null ? (options.AppUserModelId, true) : AumidHelper.GetAumid();

            if (options is null || !options.DisableComServer)
            {
                _serverUuid = options.ComActivatorGuidOverride ?? AumidHelper.GetGuidFromId(_aumid.id);
                NotificationComServer.CreateAndRegisterActivator(_serverUuid.Value);
            }

            if (_aumid.syntetic)
            {
                AumidHelper.RegisterAumid(_aumid.id, _serverUuid, options?.AppName, options?.AppIcon);
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
                var parts = split.Split('=');
                args[parts[0]] = parts[1];
            }

            if (args.ContainsKey("notificationId"))
            {
                var action = args["action"];
                var userAction = action == "user" ? args["userAction"] : null;
                var input = pairs.ContainsKey("input") ? pairs["input"] : null;
                var eventArgs = new NativeNotificationCompletedEventArgs()
                {
                    NotificationId = uint.Parse(args["notificationId"]),
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

            using var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");

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

            using (notifier)
            {
                notifier.Show(nativeNotification.CurrentNotification);
                _notifications[nativeNotification.Id] = nativeNotification;
            }
        }

        internal void Close(NativeNotification nativeNotification)
        {
            using var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");
            if(managerStatics.QueryInterface<IToastNotificationManagerStatics2>() is { } manager2)
            {
                using var tagStr = new HStringWrapper(nativeNotification.Id.ToString());
                using var groupStr = new HStringWrapper(NotificationsGroupName);

                if (_aumid.syntetic)
                {
                    using var appIdStr = new HStringWrapper(_aumid.id);
                    manager2.History.RemoveGroupedTagWithId(tagStr, groupStr, appIdStr);
                }
                else
                {
                    manager2.History.RemoveGroupedTag(tagStr, groupStr);
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

            if (!DesktopBridgeHelpers.HasPackage())
            {
                CloseAll();
            }

            if (_serverUuid is not null)
            {
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
                bitmap.Save(path);

                return new Uri(path).AbsoluteUri;
            }
            catch(Exception _)
            {
                return null;
            }
        }
    }
}
#endif
