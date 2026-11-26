#if INCLUDE_WINDOWS
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using Avalonia.Labs.Notifications.Windows.WinRT;
using IUnknown = Windows.Win32.System.Com.IUnknown;

namespace Avalonia.Labs.Notifications.Windows;

[GeneratedComClass]
[SupportedOSPlatform("windows8.0")]
internal partial class NotificationActivatorClassFactory(Guid guid) : IClassFactory
{
    private const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
    private const int E_NOINTERFACE = unchecked((int)0x80004002);

    public unsafe int CreateInstance(IntPtr pUnkOuter, Guid* riid, IntPtr* ppvObject)
    {
        *ppvObject = default;

        if (pUnkOuter != default)
            return CLASS_E_NOAGGREGATION;

        var iid = *riid;
        if (iid != guid
            && iid != typeof(INotificationActivationCallback).GUID
            && iid != IUnknown.IID_Guid)
        {
            return E_NOINTERFACE;
        }

        var unknown = NativeWinRTMethods.ComWrappers.GetOrCreateComInterfaceForObject(NotificationActivator.Instance, CreateComInterfaceFlags.None);
        try
        {
            // The caller owns the returned reference.
            var callbackIid = typeof(INotificationActivationCallback).GUID;
            return Marshal.QueryInterface(unknown, in callbackIid, out *ppvObject);
        }
        finally
        {
            Marshal.Release(unknown);
        }
    }

    public int LockServer(int fLock) => 0;
}
#endif
