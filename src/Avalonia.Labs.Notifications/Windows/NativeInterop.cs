using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Win32;

namespace Avalonia.Labs.Notifications.Windows
{
    [SupportedOSPlatform("windows")]
    internal class NativeInterop
    {
        private const string TOAST_ACTIVATED_LAUNCH_ARG = "-ToastActivated";
        private const int CLASS_E_NOAGGREGATION = -2147221232;
        private const long APPMODEL_ERROR_NO_PACKAGE = 15700L;
        private const int E_NOINTERFACE = -2147467262;
        private const int CLSCTX_LOCAL_SERVER = 4;
        private const int REGCLS_MULTIPLEUSE = 1;
        private const int S_OK = 0;
        private static readonly Guid IUnknownGuid = new Guid("00000000-0000-0000-C000-000000000046");

        public enum PackagePathType
        {
            PackagePathType_Install = 0,
            PackagePathType_Mutable = 1,
            PackagePathType_Effective = 2,
            PackagePathType_MachineExternal = 3,
            PackagePathType_UserExternal = 4,
            PackagePathType_EffectiveExternal = 5,
        }

        [DllImport("ole32.dll")]
        public static extern int CoRegisterClassObject(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            uint dwClsContext,
            uint flags,
            out uint lpdwRegister);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetApplicationUserModelId(IntPtr processHandle, ref int aumidLength, StringBuilder aumid);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern int GetPackagePathByFullName([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
            ref int pathLength, [Optional, MarshalAs(UnmanagedType.LPWStr)] StringBuilder? path);

        [ComImport]
        [Guid("00000001-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IClassFactory
        {
            [PreserveSig]
            int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject);

            [PreserveSig]
            int LockServer(bool fLock);
        }

        [RequiresUnreferencedCode("")]
        public static void CreateAndRegisterActivator()
        {
            var uuid = typeof(NotificationActivator).GUID;
            if (!IsContainerized())
                RegisterComServer(uuid, Process.GetCurrentProcess()?.MainModule?.FileName ?? "");

            CoRegisterClassObject(uuid,
                new NotificationActivatorClassFactory(),
                CLSCTX_LOCAL_SERVER,
                REGCLS_MULTIPLEUSE,
                out var c);
        }

        private class NotificationActivatorClassFactory : IClassFactory
        {
            public int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
            {
                ppvObject = IntPtr.Zero;

                if (pUnkOuter != IntPtr.Zero)
                    Marshal.ThrowExceptionForHR(CLASS_E_NOAGGREGATION);

                if (riid == typeof(NotificationActivator).GUID || riid == IUnknownGuid)
                    // Create the instance of the .NET object
                    ppvObject = Marshal.GetComInterfaceForObject(new NotificationActivator(),
                        typeof(INotificationActivationCallback));
                else
                    // The object that ppvObject points to does not support the
                    // interface identified by riid.
                    Marshal.ThrowExceptionForHR(E_NOINTERFACE);
                return S_OK;
            }

            public int LockServer(bool fLock)
            {
                return S_OK;
            }
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

        [ComImport]
        [Guid("53E31837-6600-4A81-9395-75CFFE746F94")]
        [ComVisible(true)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface INotificationActivationCallback
        {
            void Activate(
                [In][MarshalAs(UnmanagedType.LPWStr)] string appUserModelId,
                [In][MarshalAs(UnmanagedType.LPWStr)] string invokedArgs,
                [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
                NOTIFICATION_USER_INPUT_DATA[] data,
                [In][MarshalAs(UnmanagedType.U4)] uint dataCount);
        }

        [Serializable]
        public struct NOTIFICATION_USER_INPUT_DATA
        {
            [MarshalAs(UnmanagedType.LPWStr)] public readonly string Key;

            [MarshalAs(UnmanagedType.LPWStr)] public readonly string Value;
        }

        [ClassInterface(ClassInterfaceType.None)]
        [ComSourceInterfaces(typeof(INotificationActivationCallback))]
        [Guid("67890354-2A47-444C-B15F-DBF513C82F04")]
        [ComVisible(true)]
        public class NotificationActivator : INotificationActivationCallback
        {
            #region interface INotificationActivationCallback

            public void Activate(string appUserModelId, string invokedArgs, NOTIFICATION_USER_INPUT_DATA[] data,
                uint dataCount)
            {
                var pairs = Enumerable.Range(0, (int)dataCount)
                    .ToDictionary(i => data[i].Key, i => data[i].Value);

                (Notifications.NativeNotificationManager.Current as NativeNotificationManager)?.OnNotificationReceived(invokedArgs, pairs, appUserModelId);
            }

            #endregion
        }

        public static bool HasPackage()
        {
            int length = 0;
            var sb = new StringBuilder(0);
            int error = GetCurrentPackageFullName(ref length, sb);

            sb = new StringBuilder(length);
            error = GetCurrentPackageFullName(ref length, sb);

            return error != APPMODEL_ERROR_NO_PACKAGE;
        }

        public static bool IsContainerized()
        {
            if (IsWindows7OrLower)
                return false;

            int length = 0;
            var sb = new StringBuilder(0);
            GetCurrentPackageFullName(ref length, sb);

            sb = new StringBuilder(length);
            int error = GetCurrentPackageFullName(ref length, sb);

            if(error == APPMODEL_ERROR_NO_PACKAGE)
            {
                return false;
            }
            var packageName = sb.ToString();
            sb = new StringBuilder();
            length = 0;
            GetPackagePathByFullName(packageName, ref length, sb);
            if(length == 0)
            {
                return false;
            }
            sb = new StringBuilder(length);
            GetPackagePathByFullName(packageName, ref length, sb);
            var exe = Process.GetCurrentProcess()?.MainModule?.FileName ?? "";
            var packagePath = sb.ToString();

            return exe.StartsWith(packagePath);
        }

        public static string GetAumid()
        {
            var length = 0;
            var sb = new StringBuilder();
            var error = GetApplicationUserModelId(Process.GetCurrentProcess().Handle, ref length, sb);
            if (length != 0)
            {
                sb = new StringBuilder(length);
                error = GetApplicationUserModelId(Process.GetCurrentProcess().Handle, ref length, sb);
            }

            return sb.ToString();
        }

        internal static void DeleteActivatorRegistration()
        {
            var uuid = typeof(NotificationActivator).GUID;
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
