using System;
using System.Runtime.InteropServices;

namespace AppleInterop;

internal static unsafe partial class Libobjc
{
    internal const string libobjc = "/usr/lib/libobjc.dylib";
    public const string libdl = "libdl.dylib";
    public const string libSystem = "/usr/lib/libSystem.dylib";

    public static IntPtr LinkLibSystem() => dlopen(libSystem, 0);

    [LibraryImport(libobjc)]
    public static partial IntPtr _Block_copy(BlockLiteral* block);

    [LibraryImport(libSystem, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr dlsym(IntPtr handle, string symbol);
    [LibraryImport(libdl, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr dlopen(string path, int mode);

    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getClass(string className);
    [LibraryImport(libobjc)]
    public static partial IntPtr object_getClass(IntPtr id);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getMetaClass(string className);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr sel_getUid(string selector);

    [DllImport(libobjc)]
    public static extern IntPtr object_getIvar(IntPtr baseHandle, IntPtr ivar);
    [DllImport(libobjc)]
    public static extern void object_setIvar(IntPtr baseHandle, IntPtr ivar, IntPtr value);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int class_addIvar(IntPtr classHandle, string ivarName, IntPtr size, byte alignment, string types);

    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_allocateClassPair(IntPtr superclass, string selector, int extraBytes);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void objc_registerClassPair(IntPtr superclass);

    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getProtocol(string selector);
    [LibraryImport(libobjc)]
    public static partial int class_addProtocol(IntPtr basePtr, IntPtr protocol);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int class_addMethod(IntPtr basePtr, IntPtr selector, void* method, string types);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr class_getInstanceVariable(IntPtr basePtr, string variableName);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr ivar_getOffset(IntPtr ivar);

    
    [DllImport(libobjc)]
    public static extern IntPtr objc_autoreleasePoolPush();
    [DllImport(libobjc)]
    public static extern void objc_autoreleasePoolPop(IntPtr pull);

    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern int int_objc_msgSend(IntPtr basePtr, IntPtr selector);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern int int_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2, IntPtr param3);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2, IntPtr param3, int param4);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2, int param3);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2, int param3, IntPtr param4, IntPtr param5);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend(IntPtr basePtr, IntPtr selector);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend(IntPtr basePtr, IntPtr selector, int param1);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend(IntPtr basePtr, IntPtr selector, int param1, IntPtr param2);
}
