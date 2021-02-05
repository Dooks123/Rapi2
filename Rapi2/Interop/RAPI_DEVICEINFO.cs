using System;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAPI_DEVICEINFO
    {
        public Guid DeviceId;
        public int dwOsVersionMajor;
        public int dwOsVersionMinor;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrName;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrPlatform;

        public bool IsEmpty { get { return DeviceId == Guid.Empty; } }
    }
}