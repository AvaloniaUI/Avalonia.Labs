using System;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNNotificationAction : NSObject
{
    private static readonly IntPtr s_class = AppleInterop.UserNotifications.objc_getClass("UNNotificationAction");
    private static readonly IntPtr s_inputClass = AppleInterop.UserNotifications.objc_getClass("UNTextInputNotificationAction");
    private static readonly IntPtr s_actionWithIdentifier = Libobjc.sel_getUid("actionWithIdentifier:title:options:");
    private static readonly IntPtr s_actionWithIdentifierInput = Libobjc.sel_getUid("actionWithIdentifier:title:options:textInputButtonTitle:textInputPlaceholder:");

    private UNNotificationAction(IntPtr handle) : base(handle, true)
    {
    }

    public static UNNotificationAction? Create(string id, string title, int options = 4 /* Foreground */)
    {
        var idStr = CFString.Create(id);
        var titleStr = CFString.Create(title);
        var handle = Libobjc.intptr_objc_msgSend(s_class, s_actionWithIdentifier, idStr.Handle, titleStr.Handle, options);
        return handle == default ? null : new UNNotificationAction(handle);
    }

    public static UNNotificationAction? CreateTextInput(string id, string title, string? textInputButtonTitle, string? textInputPlaceholder, int options = 4 /* Foreground */)
    {
        var idStr = CFString.Create(id);
        var titleStr = CFString.Create(title);
        var textInputButtonTitleStr = CFString.Create(textInputButtonTitle);
        var textInputPlaceholderStr = CFString.Create(textInputPlaceholder);
        var handle = Libobjc.intptr_objc_msgSend(
            s_inputClass, s_actionWithIdentifierInput,
            idStr.Handle, titleStr.Handle, options,
            textInputButtonTitleStr?.Handle ?? default, textInputPlaceholderStr?.Handle ?? default);
        return handle == default ? null : new UNNotificationAction(handle);
    }
}
