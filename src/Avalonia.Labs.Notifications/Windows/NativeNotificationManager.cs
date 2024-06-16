using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.Media.Imaging;
using MicroCom.Runtime;

namespace Avalonia.Labs.Notifications.Windows
{
    [SupportedOSPlatform("windows")]
    internal class NativeNotificationManager : INativeNotificationManager, IDisposable
    {
        private readonly Dictionary<uint, INativeNotification> _notifications = new Dictionary<uint, INativeNotification>();
        private string? _aumid;

        public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

        public IDictionary<uint, INativeNotification> ActiveNotifications => _notifications;

        internal NotificationChannelManager ChannelManager { get; set; }
        public NativeNotificationManager()
        {
            ChannelManager = new NotificationChannelManager();
        }

        public void CloseAll()
        {
            using var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");
            if (managerStatics.QueryInterface<IToastNotificationManagerStatics2>() is { } manager2)
            {
                manager2.History?.Clear();
            }
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        internal void Initialize()
        {
            _aumid = NativeInterop.GetAumid();

            if (string.IsNullOrWhiteSpace(_aumid))
                return;

            NativeInterop.CreateAndRegisterActivator();
        }

        public INativeNotification? CreateNotification(string? category)
        {
            if (_aumid == null)
                return null;

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
            Debugger.Launch();
            if (appUserModelId != _aumid)
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
            using var notifier = managerStatics.CreateToastNotifier();
            notifier.Show(nativeNotification.CurrentNotification);
            _notifications[nativeNotification.Id] = nativeNotification;
        }

        internal void Close(NativeNotification nativeNotification)
        {
            using var managerStatics = NativeWinRTMethods.CreateActivationFactory<IToastNotificationManagerStatics>("Windows.UI.Notifications.ToastNotificationManager");
            if(managerStatics.QueryInterface<IToastNotificationManagerStatics2>() is { } manager2)
            {
                var tagPtr = NativeWinRTMethods.WindowsCreateString(nativeNotification.Id.ToString());
                manager2.History?.Remove(tagPtr);
                NativeWinRTMethods.WindowsDeleteString(tagPtr);
            }
            _notifications.Remove(nativeNotification.Id);
        }

        public void Dispose()
        {
            var path = GetAppDataFolderPath();
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            if (NativeInterop.IsContainerized() || string.IsNullOrWhiteSpace(_aumid))
                return;

            _notifications.Clear();

            if (!NativeInterop.HasPackage())
            {
                CloseAll();
            }

            NativeInterop.DeleteActivatorRegistration();
        }

        internal string GetAppDataFolderPath()
        {
            return _aumid == null ? "" : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", _aumid.Split("!")[0], "AppData", "Images");
        }

        internal string? SaveBitmapToAppPath(Bitmap bitmap)
        {
            try
            {
                if (GetAppDataFolderPath() is not string folder)
                    return null;
                var name = Convert.ToHexString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString() + Random.Shared.Next()))) + ".png";
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, name);
                bitmap.Save(path);

                return $"file:///{path}";
            }
            catch(Exception _)
            {
                return null;
            }
        }
    }
}
