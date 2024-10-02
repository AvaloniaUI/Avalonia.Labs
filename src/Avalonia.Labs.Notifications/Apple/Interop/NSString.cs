using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AppleInterop;

internal class NSString : NSObject
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSString");
    private static readonly IntPtr s_stringWithCharacters = Libobjc.sel_getUid("stringWithCharacters:length:");
    private static readonly IntPtr s_getUTF8String = Libobjc.sel_getUid("UTF8String");

    private NSString(IntPtr handle, bool owns) : base(false)
    {
        Handle = handle;
    }

    public static NSString FromHandle(IntPtr handle)
    {
        return new NSString(handle, false);
    }

    [return: NotNullIfNotNull(nameof(value))]
    public static unsafe NSString? Create(string? value)
    {
        if (value == null) { return null; }

        fixed (char* ptr = value)
        {
            return new NSString(Libobjc.intptr_objc_msgSend(
                s_class,
                s_stringWithCharacters,
                (IntPtr)ptr,
                new IntPtr((uint)value.Length)), true);
        }
    }

    public string? GetString()
    {
        var utf8 = Libobjc.intptr_objc_msgSend(Handle, s_getUTF8String);
        return Utf8PointerToString(utf8);
    }

    public static unsafe string? Utf8PointerToString(IntPtr utf8)
    {
        if (utf8 == IntPtr.Zero) { return null; }

        int count = 0;
        byte* ptr = (byte*)utf8;
        while (*ptr != 0)
        {
            count++;
            ptr++;
        }

        return Encoding.UTF8.GetString((byte*)utf8, count);
    }
}
