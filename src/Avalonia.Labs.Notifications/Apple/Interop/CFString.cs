using System.Runtime.InteropServices;

namespace AppleInterop;

internal class CFString : IDisposable
{
    private readonly IntPtr _cfString;

    public CFString(IntPtr cfString)
    {
        _cfString = cfString;
    }

    public string? Value
    {
        get
        {
            var utf8String = CFStringGetCStringPtr(_cfString, /* kCFStringEncodingUTF8 */ 0x08000100);
            if (utf8String != IntPtr.Zero)
            {
                return Marshal.PtrToStringUTF8(utf8String);
            }

            // CFString is not necessary backed by UTF8 string.
            // When it doesn't, developers should use CFStringGetCStringPtr.
            // It's not yet clear if BundleID can not be utf8 based at any case, and we should probably have a fallback anyway.
            // TODO
            return null;
        }
    }

    public void Dispose()
    {
        CFRelease(_cfString);
    }

    private const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/Versions/Current/CoreFoundation";

    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFStringGetCStringPtr(IntPtr cfString, long encoding);

    [DllImport(CoreFoundationLibrary)]
    private static extern void CFRelease(IntPtr ptr);
}
