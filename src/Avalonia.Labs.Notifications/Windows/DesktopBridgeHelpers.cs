#if INCLUDE_WINDOWS
using System.Diagnostics;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Avalonia.Labs.Notifications.Windows;

[SupportedOSPlatform("windows")]
internal class DesktopBridgeHelpers
{
    public static bool HasPackage() => GetCurrentPackageFullName() is not null;

    public static bool IsElevated
    {
        get
        {
            return new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }

    public static bool IsContainerized()
    {
        var packageName = GetCurrentPackageFullName();
        if (packageName is null)
            return false;

        var packagePath = GetPackagePathByFullName(packageName);
        if (packagePath is null)
            return false;

        var exe = Process.GetCurrentProcess()?.MainModule?.FileName ?? "";

        return exe.StartsWith(packagePath);
    }

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
}
#endif
