using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Microsoft.Win32;

namespace Avalonia.Labs.Notifications.Windows;

[SupportedOSPlatform("windows6.1")]
internal static class AumidHelper
{
    public static (string auimid, bool syntetic) GetAumid()
    {
        var process = Process.GetCurrentProcess();

        if (RetrievePackagedNotificationAppId(process.Handle) is { } appId)
            return (appId, false);

        if (PInvoke.GetCurrentProcessExplicitAppUserModelID(out var appIdPtr).Succeeded)
            return (appIdPtr.ToString(), true);

        var executable = process.MainModule!.FileName!;
        var fileName = Path.GetFileNameWithoutExtension(executable);
        var maxFileNameLength = Math.Min(40, fileName.Length);
        return ($"{fileName[..maxFileNameLength]}_{HashAppId(executable)[10..]}", true);
    }

    private static unsafe string? RetrievePackagedNotificationAppId(IntPtr processHandle)
    {
        var length = 0u;
        var sb = new PWSTR();
        PInvoke.GetApplicationUserModelId((HANDLE)processHandle, ref length, sb);
        if (length == 0)
            return null;

        var span = stackalloc char[(int)length];
        sb = new PWSTR(span);
        PInvoke.GetApplicationUserModelId((HANDLE)processHandle, ref length, sb);
        return sb.ToString();
    }

    public static Guid GetGuidFromId(string appId)
    {
        var buf = Encoding.UTF8.GetBytes(appId);
        var guid = new byte[16];
        if (buf.Length < 16)
        {
            Array.Copy(buf, guid, buf.Length);
        }
        else
        {
            var hash = SHA1.HashData(buf);
            // Hash is 20 bytes, but we need 16. We loose some of "uniqueness", but I doubt it will be fatal
            Array.Copy(hash, guid, 16);
        }

        return new Guid(guid);
    }

    private static string HashAppId(string appId)
    {
        var hashedBytes = SHA1.HashData(Encoding.UTF8.GetBytes(appId));
        return string.Join(string.Empty, hashedBytes.Select(b => b.ToString("X2")));
    }

    internal static void RegisterAumid(string id, Guid comServerGuid, string? appName, string? appIcon)
    {
        using (var rootKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\AppUserModelId\" + id))
        {
            rootKey.SetValue("DisplayName", appName ?? Application.Current?.Name ?? "Avalonia App");
            rootKey.SetValue("CustomActivator", string.Format("{{{0}}}", comServerGuid));
            if (appIcon != null)
            {
                rootKey.SetValue("IconUri", appIcon);
            }
            else
            {
                if (rootKey.GetValue("IconUri") != null)
                {
                    rootKey.DeleteValue("IconUri");
                }
            }
            rootKey.SetValue("IconBackgroundColor", "FFDDDDDD");
        }
    }
}
