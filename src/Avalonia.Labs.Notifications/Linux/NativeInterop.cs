using System.Runtime.InteropServices;

namespace Avalonia.Labs.Notifications.Linux
{
    internal static class NativeInterop
    {
        private const string libnotify = "libnotify.so.4";
        private const string libGlib = "libglib-2.0.so.0";
        private const string libGObject = "libgobject-2.0.so.0";
        private const string libGio = "libgio-2.0.so.0";
        private const string libPixbuf = "libgdk_pixbuf-2.0.so.0";

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool notify_init(string app_name);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool notify_is_initted();

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static void notify_uninit();

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr notify_get_server_caps();

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr notify_notification_new(string summary, string body, string icon);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr notify_notification_update(IntPtr notification, string summary, string body, string icon);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr notify_notification_set_app_name(IntPtr notification, string appName);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool notify_notification_show(IntPtr notification, ref IntPtr error);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static void notify_notification_add_action(IntPtr notification, string action, string label, Delegate callback, IntPtr user_data, IntPtr free_func);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public extern static void notify_notification_clear_actions(IntPtr notification);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public static extern void notify_notification_set_image_from_pixbuf(IntPtr notification, IntPtr image);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public static extern void notify_notification_set_icon_from_pixbuf(IntPtr notification, IntPtr icon);
        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public static extern void notify_notification_close(IntPtr notification, ref IntPtr error);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public static extern void notify_notification_set_timeout(IntPtr notification, int timeout);

        [DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
        public static extern void notify_notification_set_urgency(IntPtr notification, int urgency);

        [DllImport(libGlib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr g_bytes_new_take(byte[] data, IntPtr size);

        [DllImport(libGlib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void g_bytes_unref(IntPtr bytes);

        [DllImport(libGObject, CallingConvention = CallingConvention.Cdecl)]
        public static extern void g_object_unref(IntPtr objc);

        [DllImport(libGlib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void g_error_free(IntPtr error);

        [DllImport(libGlib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr g_main_loop_new(IntPtr context, bool isRunning);

        [DllImport(libGlib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void g_main_loop_run(IntPtr mainLoop);

        [DllImport(libGlib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void g_main_loop_quit(IntPtr mainLoop);

        [DllImport(libGio, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr g_memory_input_stream_new_from_data(IntPtr data, uint length, IntPtr destroy);

        [DllImport(libGio, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool g_input_stream_close(IntPtr stream, IntPtr cancellable, ref IntPtr error);

        [DllImport(libPixbuf, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gdk_pixbuf_new_from_stream(IntPtr stream, IntPtr cancellable, ref IntPtr error);

        [DllImport(libPixbuf, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gdk_pixbuf_new_from_file(string fileName, ref IntPtr error);

        [StructLayout(LayoutKind.Sequential)]
        public struct GError
        {
            public int Domain;
            public int ErrorCode;
            public string Message;

            public static GError FromPointer(IntPtr pointer)
            {
                return pointer == default ? default : Marshal.PtrToStructure<GError>(pointer);
            }
        }

        public enum NotifyUrgency
        {
            Low,
            Normal,
            Critical
        }
    }
}
