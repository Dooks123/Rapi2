using System;

namespace Rapi2
{
	/// <summary>
	/// Provides arguments for a device connection event.
	/// </summary>
	[Serializable]
	public class RemoteDeviceConnectEventArgs : EventArgs
	{
		/// <summary>
		/// Constructs a new instance of the <see cref="RemoteDeviceConnectEventArgs" /> class.
		/// </summary>
		internal RemoteDeviceConnectEventArgs(RemoteDevice dev)
		{
			Device = dev;
		}

		/// <summary>
		/// Gets the device that has been connected.
		/// </summary>
		public RemoteDevice Device { get; private set; }
	}
}