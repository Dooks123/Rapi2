using System;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    internal struct CEVALUNION
    {
        [FieldOffset(0)]
        public short iVal;     //@field CEVT_I2
        [FieldOffset(0)]
        public ushort uiVal;    //@field CEVT_UI2
        [FieldOffset(2)]
        private readonly short pad1;
        [FieldOffset(0)]
        public int lVal;     //@field CEVT_I4
        [FieldOffset(0)]
        public uint ulVal;    //@field CEVT_UI4
        [FieldOffset(4)]
        private readonly short pad2;
        //@field CEVT_AUTO_I4_
        [FieldOffset(0)]
        public long filetime; //@field CEVT_FILETIME 
        [FieldOffset(0)]
        public IntPtr lpwstr;   //@field CEVT_LPWSTR - Ptr to null terminated string
        [FieldOffset(0)]
        public IntPtr blob;     //@field CEVT_BLOB - DWORD count, and Ptr to bytes
                                //@field CEVT_AUTO_I8
                                //@field CEVT_RECID
                                //@field CEVT_STREAM
        [FieldOffset(0)]
        public int boolVal;  //@field CEVT_BOOL
        [FieldOffset(0)]
        public double dblVal;   //@field CEVT_R8
    }
}