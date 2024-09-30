using System.Runtime.Versioning;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Notifications;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.MicroCom;

namespace Avalonia.Labs.Notifications.Windows;

[SupportedOSPlatform("windows")]
internal class NotificationActivator : CallbackBase, INotificationActivationCallback
{
    public static NotificationActivator Instance { get; } = new();
    public static Guid PackagedGuid { get; } = new("67890354-2A47-444C-B15F-DBF513C82F04");

    [SupportedOSPlatform("windows10.0.17763.0")]
    public unsafe void Activate(IntPtr appUserModelId, IntPtr invokedArgs, NOTIFICATION_USER_INPUT_DATA* data, uint count)
    {
        (Notifications.NativeNotificationManager.Current as NativeNotificationManager)?
            .OnNotificationReceived(
                new PWSTR((char*)invokedArgs).ToString(),
                Enumerable.Range(0, (int)count)
                    .ToDictionary(i => data[i].Key.ToString(), i => data[i].Value.ToString()),
                new PWSTR((char*)appUserModelId).ToString());
    }
}
