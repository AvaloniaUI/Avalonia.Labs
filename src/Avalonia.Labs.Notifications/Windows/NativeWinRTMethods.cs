using System;
using System.Runtime.InteropServices;
using System.Threading;
using MicroCom.Runtime;

namespace Avalonia.Labs.Notifications.Windows
{
    internal static class NativeWinRTMethods
    {
        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall,
            PreserveSig = false)]
        internal static extern IntPtr WindowsCreateString(
            [MarshalAs(UnmanagedType.LPWStr)] string sourceString,
            int length);

        internal static IntPtr WindowsCreateString(string sourceString)
            => WindowsCreateString(sourceString, sourceString.Length);

        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll",
            CallingConvention = CallingConvention.StdCall, PreserveSig = false)]
        internal static extern void WindowsDeleteString(IntPtr hString);

        [DllImport("combase.dll", PreserveSig = false)]
        private static extern IntPtr RoActivateInstance(IntPtr activatableClassId);

        [DllImport("combase.dll", PreserveSig = false)]
        private static extern IntPtr RoGetActivationFactory(IntPtr activatableClassId, ref Guid iid);

        [DllImport("combase.dll", PreserveSig = false)]
        private static extern void RoInitialize(RO_INIT_TYPE initType);
        private static bool s_initialized;

        internal enum RO_INIT_TYPE
        {
            RO_INIT_SINGLETHREADED = 0, // Single-threaded application
            RO_INIT_MULTITHREADED = 1, // COM calls objects on any thread.
        }

        internal static T CreateInstance<T>(string fullName) where T : IUnknown
        {
            var s = WindowsCreateString(fullName);
            EnsureRoInitialized();
            var pUnk = RoActivateInstance(s);
            using var unk = MicroComRuntime.CreateProxyFor<IUnknown>(pUnk, true);
            WindowsDeleteString(s);
            return MicroComRuntime.QueryInterface<T>(unk);
        }

        internal static TFactory CreateActivationFactory<TFactory>(string fullName) where TFactory : IUnknown
        {
            var s = WindowsCreateString(fullName);
            EnsureRoInitialized();
            var guid = MicroComRuntime.GetGuidFor(typeof(TFactory));
            var pUnk = RoGetActivationFactory(s, ref guid);
            using var unk = MicroComRuntime.CreateProxyFor<IUnknown>(pUnk, true);
            WindowsDeleteString(s);
            return MicroComRuntime.QueryInterface<TFactory>(unk);
        }

        private static void EnsureRoInitialized()
        {
            if (s_initialized)
                return;
            RoInitialize(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA ?
                RO_INIT_TYPE.RO_INIT_SINGLETHREADED :
                RO_INIT_TYPE.RO_INIT_MULTITHREADED);
            s_initialized = true;
        }
    }
}
