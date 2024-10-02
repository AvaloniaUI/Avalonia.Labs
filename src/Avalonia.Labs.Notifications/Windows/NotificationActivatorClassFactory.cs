#if INCLUDE_WINDOWS
using System.Runtime.Versioning;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.MicroCom;
using MicroCom.Runtime;
using IUnknown = Windows.Win32.System.Com.IUnknown;

namespace Avalonia.Labs.Notifications.Windows;

[SupportedOSPlatform("windows")]
internal class NotificationActivatorClassFactory(Guid guid) : CallbackBase, IClassFactory
{
    public unsafe int CreateInstance(IntPtr pUnkOuter, Guid* riid, IntPtr* ppvObject)
    {
        *ppvObject = default;

        if (pUnkOuter != default)
            return /* CLASS_E_NOAGGREGATION */ -2147221232;

        var iid = *riid;
        if (iid == guid
            || iid == MicroComRuntime.GetGuidFor(typeof(INotificationActivationCallback))
            || iid == IUnknown.IID_Guid)
        {
            *ppvObject = NotificationActivator.Instance.GetNativeIntPtr<INotificationActivationCallback>();
            return 0;
        }

        return /* E_NOINTERFACE */ -2147467262;
    }

    public int LockServer(bool fLock) => 0;
}
#endif
