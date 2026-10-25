using System;
using System.Runtime.InteropServices;

namespace AppleInterop;

internal partial class UserNotifications
{
    private const string UserNotificationsFramework = "/System/Library/Frameworks/UserNotifications.framework/UserNotifications";

    [LibraryImport(UserNotificationsFramework, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getClass(string className);
    [LibraryImport(UserNotificationsFramework, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getProtocol(string name);
}
