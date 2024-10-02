using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal unsafe class UNUserNotificationCenterDelegate : NSObject
{
    private static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, void>
        s_willPresentNotification = &WillPresentNotification;
    private static readonly IntPtr s_class;

    static UNUserNotificationCenterDelegate()
    {
        var delegateClass = AllocateClassPair("AvaloniaUNUserNotificationCenterDelegate");

        var protocol = UserNotifications.objc_getProtocol("UNUserNotificationCenterDelegate");
        var result = Libobjc.class_addProtocol(delegateClass, protocol);
        Debug.Assert(result);

        var willPresentNotificationSel = Libobjc.sel_getUid("userNotificationCenter:willPresentNotification:withCompletionHandler:");
        result = Libobjc.class_addMethod(delegateClass, willPresentNotificationSel, s_willPresentNotification, "v@:@@@");
        Debug.Assert(result);

        s_class = delegateClass;
    }

    public UNUserNotificationCenterDelegate() : base(s_class)
    {
        Init();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void WillPresentNotification(IntPtr self, IntPtr sel, IntPtr notificationCenter, IntPtr presentNotification, IntPtr completionHandler)
    {
        var block = (BlockLiteral*)(void*)completionHandler;
        var callback = (void*)block->GetCallback();
        var options = 4; // allow UNNotificationPresentationOptionsAlert.
        ((delegate* unmanaged[Cdecl]<IntPtr, int, void>)callback)(completionHandler, options);
    }
}
