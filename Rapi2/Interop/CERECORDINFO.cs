using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct CERECORDINFO
    {
        public uint oidParent;
    }
}