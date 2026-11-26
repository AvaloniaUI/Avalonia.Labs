#if INCLUDE_WINDOWS
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using System.Threading;
using Windows.Win32;
using Windows.Win32.System.WinRT;

namespace Avalonia.Labs.Notifications.Windows;

[SupportedOSPlatform("windows8.0")]
internal static unsafe class NativeWinRTMethods
{
    private static bool s_initialized;

    internal static StrategyBasedComWrappers ComWrappers { get; } = new();

    internal static T CreateInstance<T>(string fullName) where T : class
    {
        using var fullNameStr = new HStringWrapper(fullName);
        EnsureRoInitialized();

        IInspectable* inspectable;
        PInvoke.RoActivateInstance(fullNameStr, &inspectable).ThrowOnFailure();

        return GetObjectForComInstance<T>(inspectable);
    }

    internal static TFactory CreateActivationFactory<TFactory>(string fullName) where TFactory : class
    {
        using var fullNameStr = new HStringWrapper(fullName);
        EnsureRoInitialized();

        var guid = typeof(TFactory).GUID;
        void* pUnk;
        PInvoke.RoGetActivationFactory(fullNameStr, &guid, &pUnk).ThrowOnFailure();

        return GetObjectForComInstance<TFactory>(pUnk);
    }

    private static void EnsureRoInitialized()
    {
        if (s_initialized)
            return;
        PInvoke.RoInitialize(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA ?
            RO_INIT_TYPE.RO_INIT_SINGLETHREADED :
            RO_INIT_TYPE.RO_INIT_MULTITHREADED);
        s_initialized = true;
    }

    private static T GetObjectForComInstance<T>(void* pUnk) where T : class
    {
        try
        {
            return (T)ComWrappers.GetOrCreateObjectForComInstance((nint)pUnk, CreateObjectFlags.None);
        }
        finally
        {
            // The wrapper holds its own reference.
            Marshal.Release((nint)pUnk);
        }
    }
}

[SupportedOSPlatform("windows8.0")]
internal readonly unsafe ref struct HStringWrapper
{
    private readonly HSTRING _pointer;
    public HStringWrapper(string? input)
    {
        if (input is null)
            _pointer = default;
        else
            PInvoke.WindowsCreateString(input, (uint)input.Length, out _pointer).ThrowOnFailure();
    }

    public void Dispose()
    {
        if (_pointer != 0)
            PInvoke.WindowsDeleteString(_pointer);
    }

    public static implicit operator HSTRING(HStringWrapper value) => value._pointer;
    public static implicit operator IntPtr(HStringWrapper value) => value._pointer;
}
#endif
