using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AppleInterop;

[SupportedOSPlatform("ios")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("tvos")]
internal class Bundle
{
    public static IntPtr GetMainBundle() => CFBundleGetMainBundle();

    public static string? GetMainBundleIdentifier()
    {
        var bundle = GetMainBundle();
        if (bundle == default)
            return null;

        var cfString = CFBundleGetIdentifier(bundle);
        if (cfString == default)
            return null;

        return CFString.GetString(cfString);
    }

    private const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFBundleGetMainBundle();

    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFBundleGetIdentifier(IntPtr bundle);
}
