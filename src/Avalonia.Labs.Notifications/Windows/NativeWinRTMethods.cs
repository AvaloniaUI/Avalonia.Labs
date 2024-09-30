using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.WinRT;
using MicroCom.Runtime;

namespace Avalonia.Labs.Notifications.Windows;

[SupportedOSPlatform("windows8.0")]
internal static unsafe class NativeWinRTMethods
{
    private static bool s_initialized;
    private static void EnsureRoInitialized()
    {
        if (s_initialized)
            return;
        PInvoke.RoInitialize(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA ?
            RO_INIT_TYPE.RO_INIT_SINGLETHREADED :
            RO_INIT_TYPE.RO_INIT_MULTITHREADED);
        s_initialized = true;
    }

    internal static T CreateInstance<T>(string fullName) where T : IUnknown
    {
        using var fullNameStr = new HStringWrapper(fullName);
        EnsureRoInitialized();

        IInspectable* inspectable;
        PInvoke.RoActivateInstance(fullNameStr, &inspectable).ThrowOnFailure();

        using var unk = MicroComRuntime.CreateProxyFor<IUnknown>(inspectable, true);
        return unk.QueryInterface<T>();
    }

    internal static TFactory CreateActivationFactory<TFactory>(string fullName) where TFactory : IUnknown
    {
        using var fullNameStr = new HStringWrapper(fullName);
        EnsureRoInitialized();

        var guid = MicroComRuntime.GetGuidFor(typeof(TFactory));
        PInvoke.RoGetActivationFactory(fullNameStr, guid, out var pUnk).ThrowOnFailure();

        using var unk = MicroComRuntime.CreateProxyFor<IUnknown>(pUnk, true);
        return unk.QueryInterface<TFactory>();
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
        {
            fixed (HSTRING* ptr = &_pointer)
                PInvoke.WindowsCreateString(input, (uint)input.Length, ptr).ThrowOnFailure();
        }
    }

    public void Dispose()
    {
        if (_pointer != 0)
            PInvoke.WindowsDeleteString(_pointer);
    }

    public static implicit operator HSTRING(HStringWrapper value) => value._pointer;
    public static implicit operator IntPtr(HStringWrapper value) => value._pointer;
}
