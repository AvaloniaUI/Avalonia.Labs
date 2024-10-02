using System;
using System.Collections.Generic;

namespace AppleInterop;

internal class NSArray : NSObject
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSArray");
    private static readonly IntPtr s_arrayWithObjects = Libobjc.sel_getUid("arrayWithObjects:count:");

    private NSArray(IntPtr handle) : base(handle, true)
    {
    }

    public static NSArray WithObjects(IReadOnlyList<NSObject> objects)
    {
        var handles = new IntPtr[objects.Count];
        for (int i = 0; i < objects.Count; i++)
        {
            handles[i] = objects[i].Handle;
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
