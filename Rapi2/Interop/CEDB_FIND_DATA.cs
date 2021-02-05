using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CEDB_FIND_DATA
    {
        public uint OidDb;
        public CEDBASEINFO DbInfo;
    }
}