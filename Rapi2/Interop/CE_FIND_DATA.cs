using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    internal struct CE_FIND_DATA
    {
        [FieldOffset(0)] public uint dwFileAttributes;
        [FieldOffset(4)] public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        [FieldOffset(12)] public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        [FieldOffset(20)] public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        [FieldOffset(28)] public uint nFileSizeHigh;
        [FieldOffset(32)] public uint nFileSizeLow;
        [FieldOffset(36)] public uint dwOID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260), FieldOffset(40)] public string Name;
    }
}