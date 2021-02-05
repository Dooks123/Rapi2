using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [Guid("b4fd053e-4810-46db-889b-20e638e334f0"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRAPISink
    {
        [PreserveSig]
        void OnDeviceConnected([In, MarshalAs(UnmanagedType.Interface)] IRAPIDevice pIDevice);

        [PreserveSig]
        void OnDeviceDisconnected([In, MarshalAs(UnmanagedType.Interface)] IRAPIDevice pIDevice);
    };
}