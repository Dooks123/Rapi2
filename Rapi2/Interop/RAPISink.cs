using System;

namespace Rapi2.Interop
{
    internal class RAPISink : IRAPISink
    {
        /// <summary>Occurs when device connected.</summary>
        public event EventHandler<DeviceConnectEventArgs> DeviceConnected;

        /// <summary>Occurs when device disconnected.</summary>
        public event EventHandler<DeviceConnectEventArgs> DeviceDisconnected;

        /// <summary>Raises the <see cref="E:RAPISink.DeviceConnected"/> event.</summary>
        public void OnDeviceConnected(IRAPIDevice pIDevice)
        {
            DeviceConnected?.Invoke(this, new DeviceConnectEventArgs(pIDevice));
        }

        /// <summary>Raises the <see cref="E:RAPISink.DeviceDisconnected"/> event.</summary>
        public void OnDeviceDisconnected(IRAPIDevice pIDevice)
        {
            DeviceDisconnected?.Invoke(this, new DeviceConnectEventArgs(pIDevice));
        }
    }
}