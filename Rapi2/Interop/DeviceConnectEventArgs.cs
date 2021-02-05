using System;

namespace Rapi2.Interop
{
    /// <summary>
    /// Provides arguments for a device connect/disconnect event.
    /// </summary>
    [Serializable]
    internal class DeviceConnectEventArgs : EventArgs
    {
        /// <summary>
        /// Device involved in the connect or disconnect event.
        /// </summary>
        public IRAPIDevice Device { get; private set; }

        /// <summary>
        /// Constructs a new instance of the <see cref="DeviceConnectEventArgs" /> class.
        /// </summary>
        public DeviceConnectEventArgs(IRAPIDevice device)
        {
            Device = device;
        }
    }
}