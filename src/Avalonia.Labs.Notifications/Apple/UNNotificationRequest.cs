using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNNotificationRequest : NSObject
{
    private static readonly IntPtr s_class = UserNotifications.objc_getClass("UNNotificationRequest");
    private static readonly IntPtr s_requestWithIdentifier = Libobjc.sel_getUid("requestWithIdentifier:content:trigger:");

    private UNNotificationRequest(IntPtr handle) : base(true) => Handle = handle;

    public static UNNotificationRequest FromIdentifier(
        NSString identifier,
        UNMutableNotificationContent content,
        IntPtr trigger = default)
    {
        var handle = Libobjc.intptr_objc_msgSend(s_class, s_requestWithIdentifier, identifier.Handle, content.Handle, trigger);
        if (handle == IntPtr.Zero)
            throw new InvalidOperationException($"Unable to obtain notification request with identifier {identifier}");

        return new UNNotificationRequest(handle);
    }
}
