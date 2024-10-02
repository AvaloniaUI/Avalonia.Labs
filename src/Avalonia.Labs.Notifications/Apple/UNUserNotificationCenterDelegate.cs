using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AppleInterop;

namespace Avalonia.Labs.Notifications.Apple;

internal unsafe class UNUserNotificationCenterDelegate : NSObject
{
    private readonly GCHandle _managedHandle;

    private static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, void>
        s_willPresentNotification = &OnWillPresentNotification;
    private static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, void>
        s_didReceiveNotificationResponse = &OnDidReceiveNotificationResponse;
    private static readonly IntPtr s_class;
    private static readonly IntPtr s_ivar;

    static UNUserNotificationCenterDelegate()
    {
        var delegateClass = AllocateClassPair("AvaloniaUNUserNotificationCenterDelegate");

        var protocol = UserNotifications.objc_getProtocol("UNUserNotificationCenterDelegate");
        var result = Libobjc.class_addProtocol(delegateClass, protocol);
        Debug.Assert(result == 1);

        var willPresentNotificationSel = Libobjc.sel_getUid("userNotificationCenter:willPresentNotification:withCompletionHandler:");
        result = Libobjc.class_addMethod(delegateClass, willPresentNotificationSel, s_willPresentNotification, "v@:@@@");
        Debug.Assert(result == 1);

        var didReceiveNotificationResponse = Libobjc.sel_getUid("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:");
        result = Libobjc.class_addMethod(delegateClass, didReceiveNotificationResponse, s_didReceiveNotificationResponse, "v@:@@@");
        Debug.Assert(result == 1);

        result = Libobjc.class_addIvar(delegateClass, "_managedThis", sizeof(IntPtr), 0, "@");
        Debug.Assert(result == 1);

        Libobjc.objc_registerClassPair(delegateClass);
        s_class = delegateClass;
    }

    public UNUserNotificationCenterDelegate() : base(s_class)
    {
        Init();
        _managedHandle = GCHandle.Alloc(this);
        SetIvarValue("_managedThis", GCHandle.ToIntPtr(_managedHandle));
    }

    public event EventHandler<string> WillPresentNotification;
    public event EventHandler<(string notificationId, string actionId)> DidReceiveNotificationResponse;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnWillPresentNotification(IntPtr self, IntPtr sel, IntPtr notificationCenter, IntPtr presentNotification, IntPtr completionHandler)
    {
        var managedHandle = GetIvarValue(self, "_managedThis");
        var managedThis = managedHandle == default ? null : GCHandle.FromIntPtr(managedHandle).Target as UNUserNotificationCenterDelegate;

        var id = UNNotificationRequest.GetIdentifierFromUNNotification(presentNotification);
        managedThis?.WillPresentNotification?.Invoke(managedThis, id);

        var callback = (delegate* unmanaged[Cdecl]<IntPtr, int, void>)BlockLiteral.GetCallback(completionHandler);
        var options = 4; // allow UNNotificationPresentationOptionsAlert.
        callback(completionHandler, options);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnDidReceiveNotificationResponse(IntPtr self, IntPtr sel, IntPtr notificationCenter, IntPtr notificationResponse, IntPtr completionHandler)
    {
        var managedHandle = GetIvarValue(self, "_managedThis");
        var managedThis = managedHandle == default ? null : GCHandle.FromIntPtr(managedHandle).Target as UNUserNotificationCenterDelegate;

        var notificationId = UNNotificationRequest.GetIdentifierFromUNNotificationResponse(notificationResponse);
        var actionId = UNNotificationRequest.GetActionIdentifierFromUNNotificationResponse(notificationResponse);
        managedThis?.DidReceiveNotificationResponse?.Invoke(managedThis, (notificationId, actionId));

        var callback = (delegate* unmanaged[Cdecl]<IntPtr, void>)BlockLiteral.GetCallback(completionHandler);
        callback(completionHandler);
    }
}
