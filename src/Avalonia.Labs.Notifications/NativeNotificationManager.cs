namespace Avalonia.Labs.Notifications
{
    public static class NativeNotificationManager
    {
        public static INativeNotificationManager? Current { get; private set; }

        public static void RegisterNativeNotificationManager(INativeNotificationManager nativeNotificationManager)
        {
            Current = nativeNotificationManager;
        }
    }
}
