using System;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNNotificationRequest : NSObject
{
    private readonly UNMutableNotificationContent _content;
    private static readonly IntPtr s_class = AppleInterop.UserNotifications.objc_getClass("UNNotificationRequest");
    private static readonly IntPtr s_textInputClass = AppleInterop.UserNotifications.objc_getClass("UNTextInputNotificationResponse");
    private static readonly IntPtr s_requestWithIdentifier = Libobjc.sel_getUid("requestWithIdentifier:content:trigger:");
    private static readonly IntPtr s_identifier = Libobjc.sel_getUid("identifier");
    private static readonly IntPtr s_request = Libobjc.sel_getUid("request"); // property on UNNotification
    private static readonly IntPtr s_notification = Libobjc.sel_getUid("notification"); // property on UNNotificationResponse
    private static readonly IntPtr s_actionIdentifier = Libobjc.sel_getUid("actionIdentifier"); // property on UNNotificationResponse
    private static readonly IntPtr s_userText = Libobjc.sel_getUid("userText"); // property on UNNotificationResponse

    private UNNotificationRequest(IntPtr handle, UNMutableNotificationContent content) : base(handle, true)
    {
        _content = content;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _content.Dispose();
        }
    }

    public static UNNotificationRequest FromIdentifier(
        CFString identifier,
        UNMutableNotificationContent content,
        IntPtr trigger = default)
    {
        var handle = Libobjc.intptr_objc_msgSend(s_class, s_requestWithIdentifier, identifier.Handle, content.Handle, trigger);
        if (handle == IntPtr.Zero)
            throw new InvalidOperationException($"Unable to obtain notification request with identifier {identifier}");

        return new UNNotificationRequest(handle, content);
    }

    public static string? GetIdentifierFromUNNotification(IntPtr notificationHandle)
    {
        var requestHandle = Libobjc.intptr_objc_msgSend(notificationHandle, s_request);
        var identifierHandle = Libobjc.intptr_objc_msgSend(requestHandle, s_identifier);
        return CFString.GetString(identifierHandle);
    }

    public static string? GetIdentifierFromUNNotificationResponse(IntPtr notificationResponseHandle)
    {
        var notificationHandle = Libobjc.intptr_objc_msgSend(notificationResponseHandle, s_notification);
        return GetIdentifierFromUNNotification(notificationHandle);
    }

    public static string? GetActionIdentifierFromUNNotificationResponse(IntPtr notificationResponseHandle)
    {
        var identifier = Libobjc.intptr_objc_msgSend(notificationResponseHandle, s_actionIdentifier);
        return CFString.GetString(identifier);
    }

    public static string? GetActionUserTextFromUNNotificationResponse(IntPtr notificationResponseHandle)
    {
        if (Libobjc.object_getClass(notificationResponseHandle) != s_textInputClass)
            return null;

        var identifier = Libobjc.intptr_objc_msgSend(notificationResponseHandle, s_userText);
        return CFString.GetString(identifier);
    }
}
