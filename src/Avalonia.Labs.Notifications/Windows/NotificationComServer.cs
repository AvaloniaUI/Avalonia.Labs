#if INCLUDE_WINDOWS
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.Com;
using Avalonia.Labs.Notifications.Windows.WinRT;
using MicroCom.Runtime;
using Microsoft.Win32;
using IUnknown = Windows.Win32.System.Com.IUnknown;

namespace Avalonia.Labs.Notifications.Windows;

[SupportedOSPlatform("windows8.1")]
internal class NotificationComServer
{
    private static NotificationActivatorClassFactory? s_classFactory;
    private static uint s_cookie;

    public static unsafe void CreateAndRegisterActivator(Guid uuid)
    {
        if (s_classFactory is not null)
            throw new InvalidOperationException("Already registered.");

        if (!DesktopBridgeHelpers.IsContainerized())
            RegisterComServer(uuid);

        s_classFactory = new NotificationActivatorClassFactory(uuid);
        PInvoke.CoRegisterClassObject(uuid,
            (IUnknown*)s_classFactory.GetNativeIntPtr<IClassFactory>(),
            CLSCTX.CLSCTX_LOCAL_SERVER,
            REGCLS.REGCLS_MULTIPLEUSE,
            out s_cookie).ThrowOnFailure();
    }

    public static void UnregisterActivator()
    {
        s_classFactory = null;
        PInvoke.CoRevokeClassObject(s_cookie);
    }

    private static void RegisterComServer(Guid guid)
    {
        var exePath = Process.GetCurrentProcess().MainModule!.FileName!;

        // We register the EXE to start up when the notification is activated
        var regString = string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", guid);
        using (var key = Registry.CurrentUser.CreateSubKey(regString + "\\LocalServer32"))
        {
            // Include a flag so we know this was a toast activation and should wait for COM to process
            // We also wrap EXE path in quotes for extra security
            key.SetValue(null, exePath);
        }

        if (DesktopBridgeHelpers.IsElevated)
        {
            //// For elevated apps, we need to ensure they'll activate in existing running process by adding
            //// some values in local machine
            using (var key = Registry.LocalMachine.CreateSubKey(regString))
            {
                // Same as above, except also including AppId to link to our AppId entry below
                using (var localServer32 = key.CreateSubKey("LocalServer32"))
                {
                    localServer32.SetValue(null, '"' + exePath + '"');
                }

                key.SetValue("AppId", "{" + guid + "}");
            }

            // This tells COM to match any client, so Action Center will activate our elevated process.
            // More info: https://docs.microsoft.com/windows/win32/com/runas
            using (var key = Registry.LocalMachine.CreateSubKey(string.Format("SOFTWARE\\Classes\\AppID\\{{{0}}}", guid)))
            {
                key.SetValue("RunAs", "Interactive User");
            }
        }
    }

    internal static void DeleteActivatorRegistration(Guid uuid)
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", uuid));
        }
        catch
        {
        }

        if (DesktopBridgeHelpers.IsElevated)
        {
            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", uuid));
            }
            catch
            {
            }

            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(string.Format("SOFTWARE\\Classes\\AppID\\{{{0}}}", uuid));
            }
            catch
            {
            }
        }
    }
}
#endif
