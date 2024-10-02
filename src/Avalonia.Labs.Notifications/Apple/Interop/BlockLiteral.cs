using System.Runtime.InteropServices;

namespace AppleInterop;

[StructLayout (LayoutKind.Sequential)]
internal unsafe ref struct BlockDescriptor
{
    public static IntPtr GlobalDescriptor { get; }
    static BlockDescriptor()
    {
        GlobalDescriptor = Marshal.AllocHGlobal(sizeof(BlockDescriptor));
        var descriptor = (BlockDescriptor*)(void*)GlobalDescriptor;
        descriptor->size = sizeof(BlockLiteral);
    }

    private long reserved; // always nil
    private long size; // size of the entire Block_literal
    private delegate* unmanaged[Cdecl]<void*, void*, void> copy_helper;
    private delegate* unmanaged[Cdecl]<void*, void> dispose_helper;
}

[StructLayout (LayoutKind.Sequential)]
internal ref struct BlockLiteral(IntPtr invoke)
{
    static IntPtr block_class;
    static IntPtr NSConcreteStackBlock
    {
        get
        {
            if (block_class == IntPtr.Zero)
            {
                block_class = Libobjc.dlsym (Libobjc.LinkLibSystem(), "_NSConcreteStackBlock"); // _NSConcreteGlobalBlock
            }
            return block_class;
        }
    }

    private IntPtr isa = NSConcreteStackBlock;
    private BlockFlags flags = 0;
    private int reserved;
    private IntPtr invoke = invoke;
    private IntPtr block_descriptor = BlockDescriptor.GlobalDescriptor;
    private IntPtr state;

    public static unsafe IntPtr GetCallback(IntPtr blockPtr)
    {
        var block = (BlockLiteral*)(void*)blockPtr;
        return block->invoke;
    }

    public static unsafe IntPtr TryGetBlockState(IntPtr blockPtr)
    {
        var block = (BlockLiteral*)(void*)blockPtr;
        if (block->block_descriptor == BlockDescriptor.GlobalDescriptor)
        {
            return block->state;
        }

        return default;
    }

    public static unsafe IntPtr GetBlockForFunctionPointer(IntPtr callback, IntPtr state)
    {
        var block = new BlockLiteral(callback);
        block.state = state;

        return Libobjc._Block_copy(&block);
    }

    [Flags]
    enum BlockFlags
    {
        BLOCK_REFCOUNT_MASK = (0xffff),
        BLOCK_NEEDS_FREE = (1 << 24),
        BLOCK_HAS_COPY_DISPOSE = (1 << 25),
        BLOCK_HAS_CTOR = (1 << 26), /* Helpers have C++ code. */
        BLOCK_IS_GC = (1 << 27),
        BLOCK_IS_GLOBAL = (1 << 28),
        BLOCK_HAS_DESCRIPTOR = (1 << 29), // This meaning was deprecated 
        BLOCK_HAS_STRET = (1 << 29),
        BLOCK_HAS_SIGNATURE = (1 << 30),
    }
}
