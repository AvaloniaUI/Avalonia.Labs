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
        
        // DON'T dispose CFBundleGetIdentifier. For whatever reason macOS really doesn't like it and crashes in random SkiaSharp code...?
        var cfStringWrapper = new CFString(cfString);
        return cfStringWrapper.Value;
    }

    private const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/Versions/Current/CoreFoundation";

    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFBundleGetMainBundle();

    [DllImport(CoreFoundationLibrary)]
    private static extern IntPtr CFBundleGetIdentifier(IntPtr bundle);
}
