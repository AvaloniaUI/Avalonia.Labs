using System;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNNotificationAction : NSObject
{
    private static readonly IntPtr s_class = AppleInterop.UserNotifications.objc_getClass("UNNotificationAction");
    private static readonly IntPtr s_actionWithIdentifier = Libobjc.sel_getUid("actionWithIdentifier:title:options:");

    private UNNotificationAction(IntPtr handle) : base(false)
    {
        Handle = handle;
    }

    public static UNNotificationAction? Create(string id, string title, int options = 4 /* Foreground */)
    {
        using var idStr = NSString.Create(id);
        using var titleStr = NSString.Create(title);
        var handle = Libobjc.intptr_objc_msgSend(s_class, s_actionWithIdentifier, idStr.Handle, titleStr.Handle, options);
        return handle == default ? null : new UNNotificationAction(handle);
    }
}
