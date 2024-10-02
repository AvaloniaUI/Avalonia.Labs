using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications.Linux
{
    internal class LinuxNativeNotification : INativeNotification
    {
        private readonly NotificationChannel _channel;
        private readonly LinuxNativeNotificationManager _manager;

        private static int s_currentId;

        /// <inheritdoc />
        public uint Id { get; }

        /// <inheritdoc />
        public string Category => string.Empty;

        /// <inheritdoc />
        public string? Title { get; set; }

        /// <inheritdoc />
        public string? Tag { get; set; }

        /// <inheritdoc />
        public string? Message { get; set; }

        /// <inheritdoc />
        public TimeSpan? Expiration { get; set; }

        /// <inheritdoc />
        public Bitmap? Icon { get; set; }

        /// <inheritdoc />
        public string? ReplyActionTag { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<NativeNotificationAction> Actions { get; private set; }

        public NotificationPriority Priority => _channel.Priority;

        public LinuxNativeNotification(NotificationChannel channel, LinuxNativeNotificationManager nativeNotificationManager)
        {
            _channel = channel;
            _manager = nativeNotificationManager;
            Id = (uint)GetNextId();
            Actions = channel.Actions;
        }

        /// <inheritdoc />
        public void SetActions(IReadOnlyList<NativeNotificationAction> actions)
        {
            Actions = actions;
        }

        /// <inheritdoc />
        public void Show() => _manager.ShowNotificationAsync(this);

        /// <inheritdoc />
        public void Close() => _manager.CloseNotificationAsync(this);

        private static int GetNextId() => Interlocked.Increment(ref s_currentId);
    }
}
