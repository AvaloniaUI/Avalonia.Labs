using System;
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
    private static readonly IntPtr s_conformsToProtocol = Libobjc.sel_getUid("conformsToProtocol:");

    protected NSObject(IntPtr handle, bool owns)
    {
        Handle = handle;
        _owns = owns;
        if (!owns)
        {
            Libobjc.void_objc_msgSend(Handle, s_retainSel);
        }
    }

    protected NSObject(IntPtr classHandle) : this(Libobjc.intptr_objc_msgSend(classHandle, s_allocSel), true)
    {
    }

    public IntPtr Handle { get; private set; }

    public static IntPtr AllocateClassPair(string className)
        => Libobjc.objc_allocateClassPair(s_class, className, 0);

    protected void Init()
    {
        Handle = Libobjc.intptr_objc_msgSend(Handle, s_initSel);
    }

    public static bool ConformsToProtocol(IntPtr handle, IntPtr protocolHandle)
    {
        return Libobjc.int_objc_msgSend(handle, s_conformsToProtocol, protocolHandle) == 1;
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

    private void ReleaseUnmanagedResources(bool disposing)
    {
        Libobjc.void_objc_msgSend(Handle,  s_releaseSel);
    }

    protected virtual void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources(disposing);
    }

    public void Dispose()
    {
        Dispose(true);
        Handle = default;
        GC.SuppressFinalize(this);
    }

    // Finalizer is dangerous here, as we don't know well if ObjC side still uses object or not. Be careful.
    // ~NSObject() { }
}
