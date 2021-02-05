using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
    internal struct CEDIRINFO
    {
        public uint dwAttributes;
        public uint oidParent;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDirName;
    }
}