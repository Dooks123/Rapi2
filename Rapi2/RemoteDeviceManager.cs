using System;
using System.Collections;
using System.Collections.Generic;
using Rapi2.Interop;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace Rapi2
{
    /// <summary>
    /// Manages connection status and availability of remote devices.
    /// </summary>
    /// <example>
    /// RemoteDeviceManager r = new RemoteDeviceManager();
    ///	r.DeviceDisconnected += r_DeviceDisconnected;
    ///	RemoteDevice dev = r.Devices.FirstConnectedDevice;
    ///	if (dev == null)
    ///		return;
    ///
    ///	Console.WriteLine(dev.Name + ":" + dev.Platform);
    /// </example>
    public class RemoteDeviceManager : IDisposable
    {
        [ThreadStatic]
        internal static Dictionary<Guid, RemoteDevice> deviceInstances;

        [ThreadStatic]
        private static IRAPIDesktop _iDesktop;
        private static readonly Control mainThreadPump;

        private readonly int adviceContext;
        private Thread invokeThread;
        private RAPISink iSink;

        static RemoteDeviceManager()
        {
            try
            {
                mainThreadPump = new Control();
                mainThreadPump.CreateControl();
            }
            catch { }
        }

        /// <summary>
        /// Creates a new instance of <c>RemoteDeviceManager</c>.
        /// </summary>
        public RemoteDeviceManager()
        {
            if (IRapiDesktopInstance == null)
                throw new InvalidOperationException(Properties.Resources.ErrorNoActiveSync);
            iSink = new RAPISink();
            iSink.DeviceConnected += ISink_DeviceConnected;
            iSink.DeviceDisconnected += ISink_DeviceDisconnected;
            IRapiDesktopInstance.Advise(iSink, out adviceContext);
            Devices = new RAPIDeviceList();
        }

        private static IRAPIDesktop IRapiDesktopInstance
        {
            get
            {
                Debug.WriteLine(string.Format("IN _iDesktop = {0}; TID: {1}", _iDesktop == null ? "null" : "Valid", Thread.CurrentThread.ManagedThreadId));
                if (_iDesktop == null)
                {
                    try
                    {
                        RAPI2 r2 = new RAPI2();
                        _iDesktop = (IRAPIDesktop)r2;
                        deviceInstances = new Dictionary<Guid, RemoteDevice>(5);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to get IRAPIDesktop: " + ex.ToString());
                    }
                }
                Debug.WriteLine("OUT _iDesktop = " + (_iDesktop == null ? "null" : "Valid"));
                return _iDesktop;
            }
        }

        private delegate void DeviceEventDelegate(ref RAPI_DEVICEINFO devInfo);

        /// <summary>Occurs when a device connects.</summary>
        public event EventHandler<RemoteDeviceConnectEventArgs> DeviceConnected;

        /// <summary>Occurs when a device disconnects.</summary>
        public event EventHandler<RemoteDeviceConnectEventArgs> DeviceDisconnected;

        /// <summary>Thread unsafe notice that a device has connected.</summary>
        /// <remarks>
        /// This event will always be called from a different thread than the primary thread which holds the instance of <see cref="RemoteDeviceManager"/>.
        /// If you have a reference to a <see cref="RemoteDeviceManager"/> or <see cref="RemoteDevice"/> instance, it will be invalid in the handler 
        /// method for this event. You will need to create a new instance of <see cref="RemoteDeviceManager"/> and get a <see cref="RemoteDevice"/>
        /// instance from it in order to assure that instance is valid. This event is typically only useful in console applications. For Windows Forms
        /// applications, you should use the <see cref="DeviceConnected"/> event.
        /// </remarks>
        public event EventHandler UnsafeThreadDeviceConnectedNotice;

        /// <summary>Thread unsafe notice that a device has disconnected.</summary>
        /// <remarks>
        /// This event will always be called from a different thread than the primary thread which holds the instance of <see cref="RemoteDeviceManager"/>.
        /// If you have a reference to a <see cref="RemoteDeviceManager"/> or <see cref="RemoteDevice"/> instance, it will be invalid in the handler 
        /// method for this event. You will need to create a new instance of <see cref="RemoteDeviceManager"/> and get a <see cref="RemoteDevice"/>
        /// instance from it in order to assure that instance is valid. This event is typically only useful in console applications. For Windows Forms
        /// applications, you should use the <see cref="DeviceDisconnected"/> event.
        /// </remarks>
        public event EventHandler UnsafeThreadDeviceDisconnectedNotice;

        /// <summary>
        /// Gets a list of connected <see cref="RemoteDevice"/>.
        /// </summary>
        public RAPIDeviceList Devices { get; private set; }

        /// <summary>
        /// Cleans up all internal references.
        /// </summary>
        public void Dispose()
        {
            if (adviceContext > 0 && IRapiDesktopInstance != null)
                IRapiDesktopInstance.UnAdvise(adviceContext);
            iSink.DeviceConnected -= ISink_DeviceConnected;
            iSink.DeviceDisconnected -= ISink_DeviceDisconnected;
            iSink = null;
            Devices = null;
            if (invokeThread != null && invokeThread.IsAlive)
                invokeThread.Abort();
            GC.SuppressFinalize(this);
        }

        private void ISink_DeviceConnected(object sender, DeviceConnectEventArgs e)
        {
            EventHandler h = UnsafeThreadDeviceConnectedNotice;
            if (h != null)
                try { h(this, EventArgs.Empty); } catch { }
            StartInvokeThread(new DeviceEventDelegate(DeviceConnecting), RemoteDevice.GetDeviceInfo(e.Device));
        }

        private void ISink_DeviceDisconnected(object sender, DeviceConnectEventArgs e)
        {
            EventHandler h = UnsafeThreadDeviceDisconnectedNotice;
            if (h != null)
                try { h(this, EventArgs.Empty); } catch { }
            StartInvokeThread(new DeviceEventDelegate(DeviceDisconnecting), RemoteDevice.GetDeviceInfo(e.Device));
        }

        private struct ThreadInfo
        {
            public DeviceEventDelegate method;
            public RAPI_DEVICEINFO devInfo;

            public ThreadInfo(DeviceEventDelegate m, RAPI_DEVICEINFO d)
            {
                method = m; devInfo = d;
            }
        }

        private void StartInvokeThread(DeviceEventDelegate method, RAPI_DEVICEINFO devInfo)
        {
            if (invokeThread != null && invokeThread.IsAlive)
                invokeThread.Abort();
            invokeThread = new Thread(new ParameterizedThreadStart(InvokeThreadMain));
            invokeThread.Start(new ThreadInfo(method, devInfo));
        }

        private void InvokeThreadMain(object data)
        {
            try { mainThreadPump.Invoke(((ThreadInfo)data).method, ((ThreadInfo)data).devInfo); } catch { }
        }

        /// <summary>Raises the <see cref="E:RemoteDeviceManager.DeviceConnected"/> event.</summary>
        private void OnDeviceConnected(RemoteDevice dev)
        {
            DeviceConnected?.Invoke(null, new RemoteDeviceConnectEventArgs(dev));
        }

        /// <summary>Raises the <see cref="E:RemoteDeviceManager.DeviceDisconnected"/> event.</summary>
        private void OnDeviceDisconnected(RemoteDevice dev)
        {
            DeviceDisconnected?.Invoke(null, new RemoteDeviceConnectEventArgs(dev));
        }

        private void DeviceConnecting(ref RAPI_DEVICEINFO devInfo)
        {
            // Get the matching device
            RemoteDevice rdev = Devices[devInfo.DeviceId];
            if (rdev == null)
                return;

            OnDeviceConnected(rdev);
        }

        private void DeviceDisconnecting(ref RAPI_DEVICEINFO devInfo)
        {
            if (deviceInstances.TryGetValue(devInfo.DeviceId, out RemoteDevice rdev))
                rdev.Reconfigure(ref devInfo);
            else
                rdev = new RemoteDevice(ref devInfo);
            OnDeviceDisconnected(rdev);
        }

        /// <summary>
        /// Enumerates all connected devices. Access through <see cref="Devices"/>.
        /// </summary>
        public class RAPIDeviceList : IEnumerable<RemoteDevice>, IDisposable
        {
            internal RAPIDeviceList()
            {
            }

            /// <summary>
            /// Gets the first connected device. Returns null if no devices are connected.
            /// </summary>
            public RemoteDevice FirstConnectedDevice
            {
                get
                {
                    foreach (RemoteDevice dev in this)
                        return dev;
                    return null;
                }
            }

            /// <summary>
            /// Gets the <see cref="RemoteDevice"/> with the specified id.
            /// </summary>
            internal RemoteDevice this[Guid id]
            {
                get
                {
                    RemoteDevice rdev = null;
                    foreach (RemoteDevice dev in this)
                    {
                        if (dev.DeviceId == id)
                        {
                            rdev = dev;
                            break;
                        }
                    }
                    return rdev;
                }
            }

            /// <summary>
            /// Cleans up all internal references.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Returns the strongly typed enumerator.
            /// </summary>
            /// <returns>Enumerator</returns>
            public IEnumerator<RemoteDevice> GetEnumerator()
            {
                return new RAPIDeviceEnum();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Internal enumerator for connected devices.
            /// </summary>
            public class RAPIDeviceEnum : IEnumerator<RemoteDevice>
            {
                private RemoteDevice current = null;
                private IRAPIEnumDevices iEnum;

                internal RAPIDeviceEnum()
                {
                    iEnum = IRapiDesktopInstance.EnumDevices();
                }

                /// <summary>
                /// Gets the current item in the enumeration.
                /// </summary>
                public RemoteDevice Current
                {
                    get { if (current == null) throw new InvalidOperationException(); return current; }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                /// <summary>
                /// Frees all available resources.
                /// </summary>
                public void Dispose()
                {
                    current = null;
                    iEnum = null;
                }

                /// <summary>
                /// Moves to the next device. Under all systems as of 2008, there can only be one connected device.
                /// </summary>
                /// <returns>true if a device was found. Otherwise, false.</returns>
                public bool MoveNext()
                {
                    try
                    {
                        IRAPIDevice iDev = iEnum.Next();
                        if (iDev != null)
                        {
                            Guid devID = RemoteDevice.GetDeviceInfo(iDev).DeviceId;
                            if (deviceInstances.TryGetValue(devID, out RemoteDevice devInst))
                            {
                                devInst.IDevice = iDev;
                                current = devInst;
                            }
                            else
                            {
                                deviceInstances[devID] = current = new RemoteDevice(iDev);
                            }
                        }
                        return true;
                    }
                    catch (Exception)
                    {
                        current = null;
                    }
                    return false;
                }

                /// <summary>
                /// Resets the enumeration.
                /// </summary>
                public void Reset()
                {
                    iEnum.Reset();
                    current = null;
                }
            }
        }
    }
}