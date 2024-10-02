using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AppleInterop;
using Avalonia.Threading;

namespace Avalonia.Labs.Notifications.Apple;

internal class UNUserNotificationCenter : NSObject
{
    private static readonly unsafe IntPtr s_addCallback = new((delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>)&NotificationAddCallback);
    private static readonly unsafe IntPtr s_requestAuthCallback = new((delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, void>)&RequestAuthorizationCallback);

    private static readonly IntPtr s_class = UserNotifications.objc_getClass("UNUserNotificationCenter");
    private static readonly IntPtr s_current = Libobjc.sel_getUid("currentNotificationCenter");
    private static readonly IntPtr s_delegate = Libobjc.sel_getUid("setDelegate:");
    private static readonly IntPtr s_addNotificationRequest = Libobjc.sel_getUid("addNotificationRequest:withCompletionHandler:");
    private static readonly IntPtr s_removePendingNotificationRequestsWithIdentifiers = Libobjc.sel_getUid("removePendingNotificationRequestsWithIdentifiers:");
    private static readonly IntPtr s_removeAllPendingNotificationRequests = Libobjc.sel_getUid("removeAllPendingNotificationRequests");
    private static readonly IntPtr s_requestAuthorizationWithOptions = Libobjc.sel_getUid("requestAuthorizationWithOptions:completionHandler:");
    private static readonly IntPtr s_setNotificationCategories = Libobjc.sel_getUid("setNotificationCategories:");

    private UNUserNotificationCenter(IntPtr handle) : base(false) => Handle = handle;

    private static UNUserNotificationCenter? s_currentSingleton;
    public static UNUserNotificationCenter Current
    {
        get
        {
            if (s_currentSingleton is null)
            {
                var handle = Libobjc.intptr_objc_msgSend(s_class, s_current);
                if (handle == IntPtr.Zero)
                    throw new InvalidOperationException("Unable to determine current notification center.");
                s_currentSingleton = new UNUserNotificationCenter(handle);
            }

            return s_currentSingleton;
        }
    }

    public UNUserNotificationCenterDelegate? Delegate
    {
        set
        {
            Libobjc.void_objc_msgSend(Handle, s_delegate, value?.Handle ?? default);
        }
    }

    public void SetNotificationCategories(IReadOnlyList<UNNotificationCategory> categories)
    {
        using var set = NSSet.WithObjects(categories);
        Libobjc.void_objc_msgSend(Handle, s_setNotificationCategories, set.Handle);
    }

    public async Task<bool> RequestAlertAuthorization()
    {
        var tcs = new TaskCompletionSource<bool>();
        var tcsHandle = GCHandle.Alloc(tcs);
        try
        {
            var options = 1 << 2; // UNAuthorizationOptionAlert
            var block = BlockLiteral.GetBlockForFunctionPointer(s_requestAuthCallback, GCHandle.ToIntPtr(tcsHandle));
            Libobjc.void_objc_msgSend(Handle, s_requestAuthorizationWithOptions, options, block);
            return await tcs.Task;
        }
        finally
        {
            tcsHandle.Free();
        }
    }

    public async Task Add(UNNotificationRequest request)
    {
        var tcs = new TaskCompletionSource<bool>();
        var tcsHandle = GCHandle.Alloc(tcs);
        try
        {
            var block = BlockLiteral.GetBlockForFunctionPointer(new IntPtr(s_addCallback), GCHandle.ToIntPtr(tcsHandle));
            Libobjc.void_objc_msgSend(Handle, s_addNotificationRequest, request.Handle, block);
            await tcs.Task;
        }
        finally
        {
            tcsHandle.Free();
        }
    }

    public void RemovePending(string[] identifiers)
    {
        var strings = new NSString[identifiers.Length];
        try
        {
            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = NSString.Create(identifiers[i]);
            }

            using var nsArray = NSArray.WithObjects(strings);
            Libobjc.void_objc_msgSend(Handle, s_removePendingNotificationRequestsWithIdentifiers, nsArray.Handle);
        }
        finally
        {
            foreach (var nsString in strings)
            {
                nsString?.Dispose();
            }
        }
    }

    public void RemoveAllPending()
    {
        Libobjc.void_objc_msgSend(Handle, s_removeAllPendingNotificationRequests);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void NotificationAddCallback(IntPtr thisBlock, IntPtr errorPtr)
    {
        var tcsHandlePtr = BlockLiteral.TryGetBlockState(thisBlock);
        if (tcsHandlePtr == IntPtr.Zero)
            return;
        var tcs = GCHandle.FromIntPtr(tcsHandlePtr).Target as TaskCompletionSource<bool>;

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (errorPtr != IntPtr.Zero)
            {
                using var error = new NSError(errorPtr);
                if (error.LocalizedDescription != null)
                {
                    tcs?.TrySetException(new Exception(error.LocalizedDescription));
                    return;
                }
            }

            tcs?.TrySetResult(true);
        });
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void RequestAuthorizationCallback(IntPtr thisBlock, int granted, IntPtr errorPtr)
    {
        var tcsHandlePtr = BlockLiteral.TryGetBlockState(thisBlock);
        if (tcsHandlePtr == IntPtr.Zero)
            return;
        var tcs = GCHandle.FromIntPtr(tcsHandlePtr).Target as TaskCompletionSource<bool>;

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (errorPtr != IntPtr.Zero)
            {
                using var error = new NSError(errorPtr);
                if (error.LocalizedDescription != null)
                {
                    tcs?.TrySetException(new Exception(error.LocalizedDescription));
                    return;
                }
            }

            tcs?.TrySetResult(granted == 1);
        });
    }
}
