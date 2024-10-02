using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNNotificationRequest : NSObject
{
    private static readonly IntPtr s_class = UserNotifications.objc_getClass("UNNotificationRequest");
    private static readonly IntPtr s_requestWithIdentifier = Libobjc.sel_getUid("requestWithIdentifier:content:trigger:");
    private static readonly IntPtr s_identifier = Libobjc.sel_getUid("identifier");
    private static readonly IntPtr s_request = Libobjc.sel_getUid("request"); // property on UNNotification
    private static readonly IntPtr s_notification = Libobjc.sel_getUid("notification"); // property on UNNotificationResponse
    private static readonly IntPtr s_actionIdentifier = Libobjc.sel_getUid("actionIdentifier"); // property on UNNotificationResponse

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

    public static string? GetIdentifierFromUNNotification(IntPtr notificationHandle)
    {
        var requestHandle = Libobjc.intptr_objc_msgSend(notificationHandle, s_request);
        var identifierHandle = Libobjc.intptr_objc_msgSend(requestHandle, s_identifier);
        using var str = NSString.FromHandle(identifierHandle);
        return str.GetString();
    }

    public static string? GetIdentifierFromUNNotificationResponse(IntPtr notificationResponseHandle)
    {
        var notificationHandle = Libobjc.intptr_objc_msgSend(notificationResponseHandle, s_notification);
        return GetIdentifierFromUNNotification(notificationHandle);
    }

    public static string? GetActionIdentifierFromUNNotificationResponse(IntPtr notificationResponseHandle)
    {
        var identifier = Libobjc.intptr_objc_msgSend(notificationResponseHandle, s_actionIdentifier);
        using var str = NSString.FromHandle(identifier);
        return str.GetString();
    }
}
