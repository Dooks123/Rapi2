using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
    internal struct CEDBASEINFO
    {
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDbaseName;
        public uint dwDbaseType;
        public ushort wNumRecords;
        public ushort wNumSortOrder;
        public uint dwSize;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastModified;
        //[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
        //public SortOrderDescriptor[] rgSortSpecs;
    }
}