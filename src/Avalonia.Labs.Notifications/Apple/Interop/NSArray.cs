namespace AppleInterop;

internal class NSArray : NSObject
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSArray");
    private static readonly IntPtr s_arrayWithObjects = Libobjc.sel_getUid("NSArrayWithObjects:count:");

    private NSArray(IntPtr handle) : base(true)
    {
        Handle = handle;
    }

    public static NSArray WithObjects(NSString[] strings)
    {
        var handles = new IntPtr[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            handles[i] = strings[i].Handle;
        }

        return WithObjects(handles);
    }

    public static unsafe NSArray WithObjects(IntPtr[] handles)
    {
        fixed (void* ptr = handles)
        {
            var handle = Libobjc.intptr_objc_msgSend(s_class, s_arrayWithObjects, new IntPtr(ptr), new IntPtr(handles.Length));
            if (handle is default(IntPtr))
                throw new InvalidOperationException();
            return new NSArray(handle);
        }
    }
}
