using System;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNMutableNotificationContent : NSObject
{
    private static readonly IntPtr s_class = AppleInterop.UserNotifications.objc_getClass("UNMutableNotificationContent");
    private static readonly IntPtr s_title = Libobjc.sel_getUid("title");
    private static readonly IntPtr s_setTitle = Libobjc.sel_getUid("setTitle:");
    private static readonly IntPtr s_body = Libobjc.sel_getUid("body");
    private static readonly IntPtr s_setBody = Libobjc.sel_getUid("setBody:");
    private static readonly IntPtr s_categoryIdentifier = Libobjc.sel_getUid("categoryIdentifier");
    private static readonly IntPtr s_setCategoryIdentifier = Libobjc.sel_getUid("setCategoryIdentifier:");

    public UNMutableNotificationContent() : base(s_class)
    {
        Init();
    }

    public CFString? Title
    {
        get => CFString.FromHandle(Libobjc.intptr_objc_msgSend(Handle, s_title));
        set => Libobjc.void_objc_msgSend(Handle, s_setTitle, value?.Handle ?? default);
    }

    public CFString? Body
    {
        get => CFString.FromHandle(Libobjc.intptr_objc_msgSend(Handle, s_body));
        set => Libobjc.void_objc_msgSend(Handle, s_setBody, value?.Handle ?? default);
    }

    public CFString? CategoryIdentifier
    {
        get => CFString.FromHandle(Libobjc.intptr_objc_msgSend(Handle, s_categoryIdentifier));
        set => Libobjc.void_objc_msgSend(Handle, s_setCategoryIdentifier, value?.Handle ?? default);
    }
}
