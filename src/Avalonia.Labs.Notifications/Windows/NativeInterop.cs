using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Notifications;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.MicroCom;
using MicroCom.Runtime;
using Microsoft.Win32;
using IUnknown = Windows.Win32.System.Com.IUnknown;

namespace Avalonia.Labs.Notifications.Windows
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal class NativeInterop
    {
        private const string TOAST_ACTIVATED_LAUNCH_ARG = "-ToastActivated";
        private const int CLASS_E_NOAGGREGATION = -2147221232;
        private const int E_NOINTERFACE = -2147467262;

        private static readonly NotificationActivatorClassFactory s_factory = new();
        private static readonly NotificationActivator s_activator = new();

        public static unsafe void CreateAndRegisterActivator()
        {
            var uuid = NotificationActivator.Guid;
            if (!IsContainerized())
                RegisterComServer(uuid, Process.GetCurrentProcess()?.MainModule?.FileName ?? "");

            var ptr = s_factory.GetNativeIntPtr<IClassFactory>();
            PInvoke.CoRegisterClassObject(uuid,
                (IUnknown*)ptr,
                CLSCTX.CLSCTX_LOCAL_SERVER,
                REGCLS.REGCLS_MULTIPLEUSE,
                out var c).ThrowOnFailure();
        }

        private class NotificationActivatorClassFactory : CallbackBase, IClassFactory
        {
            public unsafe int CreateInstance(IntPtr pUnkOuter, Guid* riid, IntPtr* ppvObject)
            {
                *ppvObject = default;

                if (pUnkOuter != default)
                    return CLASS_E_NOAGGREGATION;

                var iid = *riid;
                if (iid == NotificationActivator.Guid
                    || iid == MicroComRuntime.GetGuidFor(typeof(INotificationActivationCallback))
                    || iid == IUnknown.IID_Guid)
                {
                    *ppvObject = s_activator.GetNativeIntPtr<INotificationActivationCallback>();
                    return 0;
                }

                return E_NOINTERFACE;
            }

            public int LockServer(bool fLock) => 0;
        }
        private static bool IsElevated
        {
            get
            {
                return new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }

        private static void RegisterComServer(Guid guid, string exePath)
        {
            // We register the EXE to start up when the notification is activated
            string regString = string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", guid);
            using (var key = Registry.CurrentUser.CreateSubKey(regString + "\\LocalServer32"))
            {
                // Include a flag so we know this was a toast activation and should wait for COM to process
                // We also wrap EXE path in quotes for extra security
                key.SetValue(null, '"' + exePath + '"' + " " + TOAST_ACTIVATED_LAUNCH_ARG);
            }

            if (IsElevated)
            {
                //// For elevated apps, we need to ensure they'll activate in existing running process by adding
                //// some values in local machine
                using (var key = Registry.LocalMachine.CreateSubKey(regString))
                {
                    // Same as above, except also including AppId to link to our AppId entry below
                    using (var localServer32 = key.CreateSubKey("LocalServer32"))
                    {
                        localServer32.SetValue(null, '"' + exePath + '"' + " " + TOAST_ACTIVATED_LAUNCH_ARG);
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
        
        public class NotificationActivator : CallbackBase, INotificationActivationCallback
        {
            public static Guid Guid { get; } = new("67890354-2A47-444C-B15F-DBF513C82F04");

            public unsafe void Activate(IntPtr appUserModelId, IntPtr invokedArgs, NOTIFICATION_USER_INPUT_DATA* data, uint count)
            {
                (Notifications.NativeNotificationManager.Current as NativeNotificationManager)?
                    .OnNotificationReceived(
                        new PWSTR((char*)invokedArgs).ToString(),
                        Enumerable.Range(0, (int)count)
                            .ToDictionary(i => data[i].Key.ToString(), i => data[i].Value.ToString()),
                        new PWSTR((char*)appUserModelId).ToString());
            }
        }

        public static bool HasPackage() => GetCurrentPackageFullName() is not null;

        private static unsafe string? GetCurrentPackageFullName()
        {
            var length = 0u;
            var sb = new PWSTR();
            _ = PInvoke.GetCurrentPackageFullName(ref length, sb);
            if (length == 0)
                return null;

            var span = stackalloc char[(int)length];
            sb = new PWSTR(span);
            var res = PInvoke.GetCurrentPackageFullName(ref length, sb);
            if (res == 0)
                return sb.ToString();
            return null;
        }
        
        private static unsafe string? GetPackagePathByFullName(string packageFullName)
        {
            var length = 0u;
            var sb = new PWSTR();
            _ = PInvoke.GetPackagePathByFullName(packageFullName, ref length, sb);
            if (length == 0)
                return null;

            var span = stackalloc char[(int)length];
            sb = new PWSTR(span);
            var res = PInvoke.GetPackagePathByFullName(packageFullName, ref length, sb);
            if (res == 0)
                return sb.ToString();
            return null;
        } 

        public static bool IsContainerized()
        {
            if (IsWindows7OrLower)
                return false;

            var packageName = GetCurrentPackageFullName();
            if (packageName is null)
                return false;

            var packagePath = GetPackagePathByFullName(packageName);
            if (packagePath is null)
                return false;

            var exe = Process.GetCurrentProcess()?.MainModule?.FileName ?? "";

            return exe.StartsWith(packagePath);
        }

        public static unsafe string? GetAumid()
        {
            var length = 0u;
            var sb = new PWSTR();
            var processId = (HANDLE)Process.GetCurrentProcess().Handle;
            PInvoke.GetApplicationUserModelId(processId, ref length, sb);
            if (length == 0)
                return null;

            var span = stackalloc char[(int)length];
            sb = new PWSTR(span);
            PInvoke.GetApplicationUserModelId(processId, ref length, sb);
            return sb.ToString();
        }

        internal static void DeleteActivatorRegistration()
        {
            var uuid = NotificationActivator.Guid;
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}", uuid));
            }
            catch
            {
            }

            if (IsElevated)
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

        private static bool IsWindows7OrLower
        {
            get
            {
                int versionMajor = Environment.OSVersion.Version.Major;
                int versionMinor = Environment.OSVersion.Version.Minor;
                double version = versionMajor + ((double)versionMinor / 10);
                return version <= 6.1;
            }
        }
    }
}
