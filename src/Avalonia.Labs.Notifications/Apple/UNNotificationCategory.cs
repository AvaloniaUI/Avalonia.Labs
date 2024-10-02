using System;
using System.Collections.Generic;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNNotificationCategory : NSObject
{
    private readonly NSArray _actions;
    private static readonly IntPtr s_class = AppleInterop.UserNotifications.objc_getClass("UNNotificationCategory");
    private static readonly IntPtr s_categoryWithIdentifier = Libobjc.sel_getUid("categoryWithIdentifier:actions:intentIdentifiers:options:");

    private UNNotificationCategory(IntPtr handle, NSArray actions) : base(handle, true)
    {
        _actions = actions;
    }

    public static UNNotificationCategory? Create(
        string id,
        IReadOnlyList<UNNotificationAction> actions)
    {
        var idStr = CFString.Create(id);
        var nsArray = NSArray.WithObjects(actions);
        var handle = Libobjc.intptr_objc_msgSend(s_class, s_categoryWithIdentifier, idStr.Handle, nsArray.Handle, default, 0);
        return handle == default ? null : new UNNotificationCategory(handle, nsArray);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _actions.Dispose();
        }
    }
}
