using System;
using System.Runtime.InteropServices;

namespace AppleInterop;

internal static unsafe partial class Libobjc
{
    internal const string libobjc = "/usr/lib/libobjc.dylib";
    public const string libdl = "libdl.dylib";
    public const string libSystem = "/usr/lib/libSystem.dylib";

    public static IntPtr LinkLibSystem() => dlopen(libSystem, 0);

#if NET7_0_OR_GREATER
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
#else
    [DllImport(libobjc)]
    public static extern IntPtr _Block_copy(BlockLiteral* block);

    [DllImport(libSystem, CharSet = CharSet.Ansi)]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);
    [DllImport(libdl, CharSet = CharSet.Ansi)]
    public static extern IntPtr dlopen(string path, int mode);

    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getClass(string className);
    [DllImport(libobjc)]
    public static partial IntPtr object_getClass(IntPtr id);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getMetaClass(string className);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr sel_getUid(string selector);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_allocateClassPair(IntPtr superclass, string selector, int extraBytes);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getProtocol(string selector);
    [DllImport(libobjc)]
    public static extern int class_addProtocol(IntPtr basePtr, IntPtr protocol);
    [DllImport(libobjc)]
    public static extern int class_addMethod(IntPtr basePtr, IntPtr selector, void* method, string types);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr class_getInstanceVariable(IntPtr basePtr, string variableName);
    [DllImport(libobjc)]
    public static extern IntPtr ivar_getOffset(IntPtr ivar);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr object_setInstanceVariable(IntPtr basePtr, string variableName, IntPtr value);
#endif

    
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
