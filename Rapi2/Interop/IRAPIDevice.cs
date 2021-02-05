using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [Guid("8a0f1632-3905-4ca4-aea4-7e094ecbb9a7"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRAPIDevice
    {
        DeviceStatus GetConnectStat();

        void GetDeviceInfo(ref RAPI_DEVICEINFO pDevInfo);

        void GetConnectionInfo(ref RAPI_CONNECTIONINFO pConnInfo);

        IRAPISession CreateSession();
    };
}