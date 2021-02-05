using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Explicit, Size = 544, Pack = 2)]
    internal struct CEOIDINFO
    {
        [FieldOffset(0)]
        public ushort wObjType;
        [FieldOffset(4), MarshalAs(UnmanagedType.ByValArray, SizeConst = 540)]
        public byte[] inf;
    }
}