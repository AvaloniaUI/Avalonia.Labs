using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace AppleInterop;

internal sealed unsafe partial class CFString : IDisposable
{
    private readonly IntPtr _cfString;

    private CFString(IntPtr cfString)
    {
        _cfString = cfString;
    }

    public IntPtr Handle => _cfString;

    public static string? GetString(IntPtr cfString)
    {
        return FromHandle(cfString).GetString();
    }

    public static CFString FromHandle(IntPtr handle)
    {
        return new CFString(handle);
    }

    [return: NotNullIfNotNull(nameof(value))]
    public static CFString? Create(string? value)
    {
        if (value != null)
        {
            return new CFString(CFStringCreateWithCString(IntPtr.Zero, value, /* kCFStringEncodingUTF8 */ 0x08000100));
        }
        else return null;
    }

    public string? GetString()
    {
        if (_cfString == IntPtr.Zero)
            return null;

        var utf8String = CFStringGetCStringPtr(_cfString, /* kCFStringEncodingUTF8 */ 0x08000100);
        if (utf8String != IntPtr.Zero)
        {
            return Marshal.PtrToStringUTF8(utf8String);
        }

        var len = (int)CFStringGetLength(_cfString);
        return string.Create(len, (len, _cfString), static (span, tuple) =>
        {
            var alloc = Marshal.AllocHGlobal(tuple.len * 8);
            try
            {
                CFStringGetCString(tuple._cfString, (byte*)alloc, (uint)tuple.len + 1, /* kCFStringEncodingUTF8 */ 0x08000100);
                Encoding.UTF8.GetChars(new ReadOnlySpan<byte>((void*)alloc, tuple.len), span);
            }
            finally
            {
                Marshal.FreeHGlobal(alloc);
            }
        });
    }

    public void Dispose()
    {
        CFRelease(_cfString);
    }

    private const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFStringGetCStringPtr(IntPtr cfString, long encoding);

    [DllImport(CoreFoundationLibrary)]
    private static extern long CFStringGetLength(IntPtr cfString);

    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFStringGetCString(IntPtr cfString, byte* str, uint length, long encoding);

    [DllImport(CoreFoundationLibrary)]
    private static extern void CFRelease(IntPtr ptr);

#if NET7_0_OR_GREATER
    [LibraryImport(CoreFoundationLibrary, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr CFStringCreateWithCString(IntPtr cfString, string str, long encoding);
#else
    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFStringCreateWithCString(IntPtr cfString, string str, long encoding);
#endif
}
