using System;
using System.Collections.Generic;

namespace AppleInterop;

internal class NSSet : NSObject
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSSet");
    private static readonly IntPtr s_arrayWithObjects = Libobjc.sel_getUid("setWithObjects:count:");

    private NSSet(IntPtr handle) : base(handle, true)
    {
    }

    public static NSSet WithObjects(IReadOnlyList<NSObject> objects)
    {
        var handles = new IntPtr[objects.Count];
        for (int i = 0; i < objects.Count; i++)
        {
            handles[i] = objects[i].Handle;
        }

        return WithObjects(handles);
    }

    public static unsafe NSSet WithObjects(IntPtr[] handles)
    {
        fixed (void* ptr = handles)
        {
            var handle = Libobjc.intptr_objc_msgSend(s_class, s_arrayWithObjects, new IntPtr(ptr), new IntPtr(handles.Length));
            if (handle is default(IntPtr))
                throw new InvalidOperationException();
            return new NSSet(handle);
        }
    }
}
