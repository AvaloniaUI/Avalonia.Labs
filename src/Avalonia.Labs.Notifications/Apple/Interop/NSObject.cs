using System.Diagnostics;

namespace AppleInterop;

internal abstract class NSObject : IDisposable
{
    private readonly bool _owns;
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSObject");
    private static readonly IntPtr s_allocSel = Libobjc.sel_getUid("alloc");
    private static readonly IntPtr s_initSel = Libobjc.sel_getUid("init");
    private static readonly IntPtr s_retainSel = Libobjc.sel_getUid("retain");
    private static readonly IntPtr s_releaseSel = Libobjc.sel_getUid("release");
    private static readonly IntPtr s_deallocSel = Libobjc.sel_getUid("dealloc");
    private static readonly IntPtr s_autoreleaseSel = Libobjc.sel_getUid("autorelease");
    private static readonly IntPtr s_retainCountSel = Libobjc.sel_getUid("retainCount");

    protected NSObject(bool owns)
    {
        _owns = owns;
        if (!_owns)
            Libobjc.void_objc_msgSend(Handle, s_retainSel);
    }

    protected NSObject(IntPtr classHandle) : this(true)
    {
        Handle = Libobjc.intptr_objc_msgSend(classHandle, s_allocSel);
    }

    public IntPtr Handle { get; protected set; }

    public static IntPtr AllocateClassPair(string className)
        => Libobjc.objc_allocateClassPair(s_class, className, 0);

    protected void Init()
    {
        Handle = Libobjc.intptr_objc_msgSend(Handle, s_initSel);
    }

    protected unsafe bool SetIvarValue(string varName, IntPtr value)
    {
        var ivarPtr = GetIvarPointer(Handle, varName);
        if (ivarPtr == default)
            return false;
        *(IntPtr*)ivarPtr = value;
        return true;
    }

    protected IntPtr GetIvarValue(string varName)
    {
        return GetIvarValue(Handle, varName);
    }

    protected static unsafe IntPtr GetIvarValue(IntPtr handle, string varName)
    {
        var ivarPtr = GetIvarPointer(handle, varName);
        if (ivarPtr == default)
            return default;
        return *(IntPtr*)ivarPtr;
    }

    private static IntPtr GetIvarPointer(IntPtr baseHandle, string varName)
    {
        var ivar = Libobjc.class_getInstanceVariable(Libobjc.object_getClass(baseHandle), varName);
        if (ivar == default) return 0;
        return baseHandle + Libobjc.ivar_getOffset(ivar);
    }

    private void ReleaseUnmanagedResources()
    {
        //Libobjc.void_objc_msgSend(Handle, s_releaseSel);
    }

    protected virtual void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
    }

    public void Dispose()
    {
        Dispose(true);
        Handle = default;
        GC.SuppressFinalize(this);
    }

#if DEBUG
    ~NSObject()
    {
        Debug.Fail("NSObject has to be manually disposed.");
    }
#endif
}
