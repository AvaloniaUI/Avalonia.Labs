#if INCLUDE_WINDOWS
using System;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Notifications;
using Avalonia.Labs.Notifications.Windows.WinRT;

namespace Avalonia.Labs.Notifications.Windows;

[GeneratedComClass]
[SupportedOSPlatform("windows")]
internal partial class NotificationActivator : INotificationActivationCallback
{
    public static NotificationActivator Instance { get; } = new();

    [SupportedOSPlatform("windows10.0.17763.0")]
    public unsafe void Activate(IntPtr appUserModelId, IntPtr invokedArgs, IntPtr userInputData, uint dataCount)
    {
        var data = (NOTIFICATION_USER_INPUT_DATA*)userInputData;
        (Notifications.NativeNotificationManager.Current as NativeNotificationManager)?
            .OnNotificationReceived(
                new PWSTR((char*)invokedArgs).ToString(),
                Enumerable.Range(0, (int)dataCount)
                    .ToDictionary(i => data[i].Key.ToString(), i => data[i].Value.ToString()),
                new PWSTR((char*)appUserModelId).ToString());
    }
}
#endif
