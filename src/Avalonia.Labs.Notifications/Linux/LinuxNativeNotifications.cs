using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Avalonia.Media.Imaging;
using static Avalonia.Labs.Notifications.Linux.NativeInterop;

namespace Avalonia.Labs.Notifications.Linux
{
    internal class LinuxNativeNotification : INativeNotification
    {
        private static int s_currentId = 0;
        private IntPtr _pixbuf;

        public uint Id { get; }

        public string Category => _channel.Id;

        public string? Title { get; set; }

        public string? Tag { get; set; }
        public string? Message { get; set; }
        public TimeSpan? Expiration { get; set; }

        public Bitmap? Icon { get; set; }

        public IReadOnlyList<NativeNotificationAction> Actions { get; private set; }
        public string? ReplyActionTag { get; set; }
        public IntPtr NativeHandle { get; internal set; }

        private readonly NotificationChannel _channel;
        private readonly LinuxNativeNotificationManager _manager;

        public LinuxNativeNotification(NotificationChannel channel, LinuxNativeNotificationManager nativeNotificationManager)
        {
            _channel = channel;
            _manager = nativeNotificationManager;
            Id = (uint)GetNextId();

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

        public unsafe void Show()
        {
            IntPtr error = IntPtr.Zero;

            _pixbuf = default;
            if (Icon != null)
            {
                using var mem = new MemoryStream();
                Icon.Save(mem);

                var data = mem.ToArray();

                fixed (byte* dataPtr = data)
                {
                    var gStream = g_memory_input_stream_new_from_data((IntPtr)dataPtr, (uint)data.Length, IntPtr.Zero);

                    _pixbuf = gdk_pixbuf_new_from_stream(gStream, IntPtr.Zero, ref error);

                    if (error != default)
                        g_error_free(error);
                    g_input_stream_close(gStream, IntPtr.Zero, ref error);
                    if (error != default)
                        g_error_free(error);
                }
            }
            var appIcon = _manager.AppIcon;
            var handle = NativeHandle != default ? NativeHandle : notify_notification_new(Title ?? "", Message ?? "", appIcon ?? "");
            if (NativeHandle != default)
            {
                notify_notification_update(NativeHandle, Title ?? "", Message ?? "", appIcon ?? "");
            }

            if(!string.IsNullOrWhiteSpace(_manager.AppName))
            {
                notify_notification_set_app_name(handle, _manager.AppName);
            }

            notify_notification_set_image_from_pixbuf(handle, _pixbuf);

            notify_notification_clear_actions(handle);
            notify_notification_add_action(handle, LinuxNativeNotificationManager.DefaultAction, "default", _manager.Callback, (IntPtr)Id, IntPtr.Zero);

            var urgency = _channel.Priority switch
            {
                NotificationPriority.Default => NotifyUrgency.Normal,
                NotificationPriority.Low => NotifyUrgency.Low,
                NotificationPriority.High => NotifyUrgency.Critical,
                NotificationPriority.Max => NotifyUrgency.Critical,
                _ => throw new NotImplementedException(),
            };

            notify_notification_set_urgency(handle, (int)urgency);

            if (Actions != null)
            {
                foreach (var action in Actions)
                {
                    notify_notification_add_action(handle, action.Tag ?? "", action.Caption ?? "", _manager.Callback, (IntPtr)Id, IntPtr.Zero);
                }
            }

            if (Expiration is { } expiration)
            {
                var ms = (int)expiration.TotalMilliseconds;

                notify_notification_set_timeout(handle, ms);
            }

            error = IntPtr.Zero;

            NativeHandle = handle;

            _manager.Show(this);
        }

        internal void DeleteNativeResources()
        {
            if(_pixbuf != default)
            {
                g_object_unref(_pixbuf);
            }

            _pixbuf = default;
        }

        private static int GetNextId()
        {
            return Interlocked.Increment(ref s_currentId);
        }
    }
}
