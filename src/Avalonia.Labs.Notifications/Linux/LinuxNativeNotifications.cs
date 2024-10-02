#if INCLUDE_LINUX
using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications.Linux
{
    internal class LinuxNativeNotification(NotificationChannel channel, LinuxNativeNotificationManager nativeNotificationManager) : INativeNotification
    {
        private static int s_currentId;

        /// <inheritdoc />
        public uint Id { get; } = (uint)GetNextId();

        /// <inheritdoc />
        public string Category => channel.Id;

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
        public IReadOnlyList<NativeNotificationAction> Actions { get; private set; } = channel.Actions;

        public NotificationPriority Priority => channel.Priority;

        /// <inheritdoc />
        public void SetActions(IReadOnlyList<NativeNotificationAction> actions)
        {
            Actions = actions;
        }

        /// <inheritdoc />
        public async void Show() => await nativeNotificationManager.ShowNotificationAsync(this);

        /// <inheritdoc />
        public async void Close() => await nativeNotificationManager.CloseNotificationAsync(this);

        private static int GetNextId() => Interlocked.Increment(ref s_currentId);
    }
}
#endif
