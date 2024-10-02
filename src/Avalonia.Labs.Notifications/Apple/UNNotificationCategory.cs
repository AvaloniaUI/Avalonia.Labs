using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNNotificationCategory : NSObject
{
    private static readonly IntPtr s_class = UserNotifications.objc_getClass("UNNotificationCategory");
    private static readonly IntPtr s_categoryWithIdentifier = Libobjc.sel_getUid("categoryWithIdentifier:actions:intentIdentifiers:options:");

    private UNNotificationCategory(IntPtr handle) : base(false)
    {
        Handle = handle;
    }

    public static UNNotificationCategory? Create(
        string id,
        IReadOnlyList<UNNotificationAction> actions)
    {
        using var idStr = NSString.Create(id);
        using var nsArray = NSArray.WithObjects(actions);
        var handle = Libobjc.intptr_objc_msgSend(s_class, s_categoryWithIdentifier, idStr.Handle, nsArray.Handle, default, 0);
        return handle == default ? null : new UNNotificationCategory(handle);
    }
}
