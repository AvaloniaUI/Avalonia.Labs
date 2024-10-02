using System;

namespace AppleInterop;

internal class NSError(IntPtr handle) : NSObject(handle, true)
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSError");
    private static readonly IntPtr s_localizedDescription = Libobjc.sel_getUid("localizedDescription");

    public string? LocalizedDescription =>
        CFString.GetString(Libobjc.intptr_objc_msgSend(Handle, s_localizedDescription));
}
