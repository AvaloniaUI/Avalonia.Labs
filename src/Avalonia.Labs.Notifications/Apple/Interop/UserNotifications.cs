using System.Runtime.InteropServices;

namespace AppleInterop;

internal partial class UserNotifications
{
    private const string UserNotificationsFramework = "/System/Library/Frameworks/UserNotifications.framework/UserNotifications";

#if NET7_0_OR_GREATER
    [LibraryImport(UserNotificationsFramework, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getClass(string className);
    [LibraryImport(UserNotificationsFramework, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getProtocol(string name);
#else
    [DllImport(UserNotificationsFramework, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getClass(string className);
    [DllImport(UserNotificationsFramework, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getProtocol(string name);
#endif
}
