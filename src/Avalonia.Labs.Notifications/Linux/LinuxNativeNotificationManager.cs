#if INCLUDE_LINUX
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.Labs.Notifications.Linux
{
    [SupportedOSPlatform("linux")]
    internal class LinuxNativeNotificationManager : INativeNotificationManagerImpl
    {
        // Clicking the notification body only emits ActionInvoked if "default-action" is set to a non-"app." action.
        private const string DefaultActionName = "default";

        private readonly Dictionary<uint, INativeNotification> _notifications = new();
        private readonly OrgFreedesktopPortalNotificationProxy _freedesktopPortalNotification = new(DBusConnection.Session.AsConnection(), "org.freedesktop.portal.Desktop", "/org/freedesktop/portal/desktop");

        private bool _isAvailable;
        private IDisposable? _signalWatcher;

        /// <inheritdoc />
        public IReadOnlyDictionary<uint, INativeNotification> ActiveNotifications => _notifications;

        public NotificationChannelManager ChannelManager { get; } = new();
        public bool ClearOnClose { get; set; }

        public async void Initialize(AppNotificationOptions? options)
        {
            if (options?.AppId is { Length: > 0 } appId)
            {
                // Must happen before any other portal call on this connection.
                try
                {
                    var registry = new OrgFreedesktopHostPortalRegistryProxy(DBusConnection.Session.AsConnection(), "org.freedesktop.portal.Desktop", "/org/freedesktop/portal/desktop");
                    await registry.RegisterAsync(appId, new Dictionary<string, VariantValue>());
                }
                catch
                {
                    // Portal is too old, the app is sandboxed, or no matching .desktop file is installed.
                    // Fall back to the app ID inferred by the portal.
                }
            }

            try
            {
                await _freedesktopPortalNotification.GetVersionPropertyAsync();
                _isAvailable = true;
            }
            catch
            {
                return;
            }

            _signalWatcher = await _freedesktopPortalNotification.WatchActionInvokedAsync((e, invoked) =>
            {
                if (e is not null || !uint.TryParse(invoked.Id, NumberStyles.None, NumberFormatInfo.InvariantInfo, out var id))
                    return;
                var isActivated = invoked.Action == DefaultActionName;
                var args = new NativeNotificationCompletedEventArgs
                {
                    NotificationId = id,
                    IsActivated = isActivated,
                    ActionTag = isActivated ? null : invoked.Action
                };

                NotificationCompleted?.Invoke(this, args);
            });
        }

        /// <inheritdoc />
        public INativeNotification? CreateNotification(string? category)
        {
            if (!_isAvailable)
                return null;

            var channel = ChannelManager.GetChannel(category ?? NotificationChannelManager.DefaultChannel) ??
                          ChannelManager.AddChannel(new NotificationChannel(NotificationChannelManager.DefaultChannel, NotificationChannelManager.DefaultChannelLabel));

            return new LinuxNativeNotification(channel, this);
        }

        /// <inheritdoc />
        public void CloseAll()
        {
            foreach (var notification in ActiveNotifications)
                notification.Value.Close();
        }

        /// <inheritdoc />
        public event EventHandler<NativeNotificationCompletedEventArgs>? NotificationCompleted;

        internal async Task ShowNotificationAsync(LinuxNativeNotification notification)
        {
            var serializedNotification = new Dictionary<string, VariantValue>
            {
                { "title", VariantValue.String(notification.Title ?? string.Empty) },
                { "body", VariantValue.String(notification.Message ?? string.Empty) },
                { "priority", VariantValue.String(NotificationPriorityToDBusPriority(notification.Priority)) },
                { "default-action", VariantValue.String(DefaultActionName) }
            };

            if (notification.Icon is not null)
            {
                using var memStream = new MemoryStream(notification.Icon.PixelSize.Width * notification.Icon.PixelSize.Height * 4);
                notification.Icon.Save(memStream);
                var iconData = memStream.ToArray();
                // Serialized GBytesIcon: (sv) with "bytes" and the encoded image as ay.
                var icon = VariantValue.Struct(VariantValue.String("bytes"), VariantValue.Variant(VariantValue.Array(iconData)));
                serializedNotification.Add("icon", icon);
            }

            if (notification.Actions.Count != 0)
            {
                var buttons = notification.Actions
                    .Select(static action =>
                        new Dict<string, VariantValue>
                        {
                            { "label", VariantValue.String(action.Caption) },
                            { "action", VariantValue.String(action.Tag) }
                        }.AsVariantValue())
                    .ToArray();

                serializedNotification.Add("buttons", VariantValue.ArrayOfVariant(buttons));
            }

            _notifications.Add(notification.Id, notification);
            await _freedesktopPortalNotification.AddNotificationAsync(notification.Id.ToString(NumberFormatInfo.InvariantInfo), serializedNotification);
        }

        internal async Task CloseNotificationAsync(LinuxNativeNotification notification)
        {
            await _freedesktopPortalNotification.RemoveNotificationAsync(notification.Id.ToString(NumberFormatInfo.InvariantInfo));
            _notifications.Remove(notification.Id);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (ClearOnClose)
                CloseAll();

            _signalWatcher?.Dispose();
        }

        private static string NotificationPriorityToDBusPriority(NotificationPriority priority) => priority switch
        {
            NotificationPriority.Default => "normal",
            NotificationPriority.Low => "low",
            NotificationPriority.High => "high",
            NotificationPriority.Max => "urgent",
            _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
        };
    }
}
#endif
