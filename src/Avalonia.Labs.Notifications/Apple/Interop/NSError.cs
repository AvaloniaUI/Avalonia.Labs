namespace AppleInterop;

internal class NSError : NSObject
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSError");
    private static readonly IntPtr s_localizedDescription = Libobjc.sel_getUid("localizedDescription");

    public NSError(IntPtr handle) : base(true)
    {
        Handle = handle;
    }

    public string? LocalizedDescription
    {
        get
        {
            using var nsString = NSString.FromHandle(Libobjc.intptr_objc_msgSend(Handle, s_localizedDescription));
            return nsString.GetString();
        }
    }
}
