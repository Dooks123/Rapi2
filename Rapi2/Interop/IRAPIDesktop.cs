using System;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [Guid("dcbeb807-14d0-4cbd-926c-b991f4fd1b91"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRAPIDesktop
    {
        void FindDevice(
            ref Guid pDeviceID,
            RAPI_GETDEVICEOPCODE opFlags,
            [Out, MarshalAs(UnmanagedType.Interface)] out IRAPIDevice ppIDevice);

        IRAPIEnumDevices EnumDevices();

        void Advise(
            [In, MarshalAs(UnmanagedType.Interface)] IRAPISink pISink,
            out int pdwContext);

        void UnAdvise(
            int dwContext);
    };
}