using Microsoft.Win32;
using Rapi2.Interop;
using Rapi2.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Rapi2
{
    /// <summary>
    /// Represents a remote device. This can only be accessed through <see cref="RemoteDeviceManager.Devices"/>.
    /// </summary>
    public class RemoteDevice : IDisposable
    {
        private const int MAX_PATH = 260;

        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

        private RAPI_DEVICEINFO cache = new RAPI_DEVICEINFO();
        private IRAPIDevice iDevice;
        private IRAPISession iSession;
        private bool sessionFailure = false;

        internal RemoteDevice(IRAPIDevice iDev)
        {
            IDevice = iDev;
        }

        internal RemoteDevice(ref RAPI_DEVICEINFO di)
        {
            cache = di;
        }

        /// <summary>
        /// Gets the means by which the device is connected.
        /// </summary>
        public ConnectionType ConnectionType
        {
            get { return ConnInfo.connType; }
        }

        /// <summary>
        /// Gets an enumerated list of all the databases on the device.
        /// </summary>
        /// <value>The databases.</value>
        public RemoteDatabaseList Databases
        {
            get { return new RemoteDatabaseList(ISession, 0); }
        }

        /// <summary>
        /// Gets the unique identifier for the device.
        /// </summary>
        public Guid DeviceId
        {
            get { return DevInfo.DeviceId; }
        }

        /// <summary>Reads the remote device's registry base key HKEY_CLASSES_ROOT.</summary>
        public DeviceRegistryKey DeviceRegistryClassesRoot
        {
            get { return new DeviceRegistryKey(ISession, 0x80000000, "HKEY_CLASSES_ROOT"); }
        }

        /// <summary>Reads the remote device's registry base key HKEY_CURRENT_USER.</summary>
        public DeviceRegistryKey DeviceRegistryCurrentUser
        {
            get { return new DeviceRegistryKey(ISession, 0x80000001, "HKEY_CURRENT_USER"); }
        }

        /// <summary>Reads the remote device's registry base key HKEY_LOCAL_MACHINE.</summary>
        public DeviceRegistryKey DeviceRegistryLocalMachine
        {
            get { return new DeviceRegistryKey(ISession, 0x80000002, "HKEY_LOCAL_MACHINE"); }
        }

        /// <summary>Reads the remote device's registry base key HKEY_USERS.</summary>
        public DeviceRegistryKey DeviceRegistryUsers
        {
            get { return new DeviceRegistryKey(ISession, 0x80000003, "HKEY_USERS"); }
        }

        /// <summary>
        /// Gets the host address of the connected desktop.
        /// </summary>
        public IPEndPoint HostAddress
        {
            get { return ConnInfo.host; }
        }

        /// <summary>
        /// Gets the assigned address for the device.
        /// </summary>
        public IPEndPoint IPAddress
        {
            get { return ConnInfo.addr; }
        }

        /// <summary>
        /// Gets the last error raised by the device.
        /// </summary>
        public int LastError
        {
            get { return ISession.CeGetLastError(); }
        }

        /// <summary>
        /// Gets the <see cref="MemoryStatus"/> for the device.
        /// </summary>
        public MemoryStatus MemoryStatus
        {
            get
            {
                MemoryStatus stat = new MemoryStatus();
                ISession.CeGlobalMemoryStatus(ref stat);
                return stat;
            }
        }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name
        {
            get { return DevInfo.bstrName; }
        }

        /// <summary>
        /// Gets the version of the device OS.
        /// </summary>
        public Version OSVersion
        {
            get
            {
                CEOSVERSIONINFO ver = new CEOSVERSIONINFO();
                ISession.CeGetVersionEx(ref ver);
                return new Version(ver.dwMajorVersion, ver.dwMinorVersion,
                    ver.dwBuildNumber, 0);
            }
        }

        /// <summary>
        /// Gets a string representation of the device platform.
        /// </summary>
        public string Platform
        {
            get { return DevInfo.bstrPlatform; }
        }

        /// <summary>
        /// Gets the <see cref="PowerStatus"/> for the device.
        /// </summary>
        public PowerStatus PowerStatus
        {
            get
            {
                PowerStatus stat = new PowerStatus();
                ISession.CeGetSystemPowerStatusEx(ref stat, 1);
                return stat;
            }
        }

        /// <summary>
        /// Gets the connection status of the device.
        /// </summary>
        public DeviceStatus Status
        {
            get { return (IDevice == null) ? DeviceStatus.Disconnected : IDevice.GetConnectStat(); }
        }

        /// <summary>
        /// Gets the <see cref="StoreInfo"/> with information about the object store on the device.
        /// </summary>
        public StoreInfo StoreInfo
        {
            get
            {
                StoreInfo stat = new StoreInfo();
                ISession.CeGetStoreInformation(ref stat);
                return stat;
            }
        }

        /// <summary>
        /// Gets the <see cref="SystemInformation"/> for the device.
        /// </summary>
        public SystemInformation SystemInformation
        {
            get
            {
                SystemInformation si = new SystemInformation();
                ISession.CeGetSystemInfo(ref si);
                return si;
            }
        }

        internal IRAPIDevice IDevice
        {
            get { return iDevice; }
            set
            {
                ResetSession();
                iDevice = value;
            }
        }

        internal IRAPISession ISession
        {
            get
            {
                if (sessionFailure)
                    throw new InvalidOperationException(Properties.Resources.ErrorDisconnectedDevice);

                if (iSession == null)
                {
                    try { iSession = IDevice.CreateSession(); }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine("ISession: " + ex.ToString()); }

                    if (iSession == null)
                    {
                        sessionFailure = true;
                        throw new InvalidOperationException(Properties.Resources.ErrorDisconnectedDevice);
                    }

                    iSession.CeRapiInit();
                }

                return iSession;
            }
        }

        private RAPIConnectionInfo ConnInfo
        {
            get
            {
                RAPI_CONNECTIONINFO ci = new RAPI_CONNECTIONINFO();
                IDevice.GetConnectionInfo(ref ci);
                return new RAPIConnectionInfo(ref ci);
            }
        }

        private RAPI_DEVICEINFO DevInfo
        {
            get { return iDevice == null ? cache : GetDeviceInfo(IDevice); }
        }

        /// <summary>
        /// This method compares a specified string to the system password on a remote device.
        /// </summary>
        /// <param name="pwd">Password to compare with the system password.</param>
        /// <returns>true if password matches. Otherwise false.</returns>
        public bool CheckPassword(string pwd)
        {
            return Convert.ToBoolean(ISession.CeCheckPassword(pwd));
        }

        /// <summary>
        /// Runs a program on a remote device. It creates a new process and its primary thread. The new process executes the specified executable file.
        /// </summary>
        /// <param name="applicationName">String that specifies the module to execute. <para>The string can specify the full path and file name of the module to execute or it can specify just the module name. In the case of a partial name, the function uses the current drive and current directory to complete the specification.</para></param>
        /// <param name="commandLine">String that specifies the command line arguments with which the application will be executed. 
        /// <para>The commandLine parameter can be NULL. In that case, the method uses the string pointed to by applicationName as the command line.</para>
        /// <para>If commandLine is non-NULL, applicationName specifies the module to execute, and commandLine specifies the command line arguments.</para></param>
        /// <param name="creationFlags">Optional conditions for creating the process.</param>
        public void CreateProcess(string applicationName, string commandLine, ProcessCreationFlags creationFlags)
        {
            if (0 == ISession.CeCreateProcess(applicationName, commandLine, 0, 0, 0, (int)creationFlags, 0, 0, 0, out _))
                ThrowRAPIException();
        }

        /// <summary>
        /// Creates a shortcut file on the device in the specified location.
        /// </summary>
        /// <param name="shortcutFileName">Name of the shortcut file.</param>
        /// <param name="targetFileName">Name of the target file.</param>
        public void CreateShortcut(string shortcutFileName, string targetFileName)
        {
            if (0 == ISession.CeSHCreateShortcut(shortcutFileName, targetFileName))
                ThrowRAPIException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IDevice = null;
            ResetSession();
        }

        /// <summary>
        /// Retrieves device-specific information about a remote device.
        /// </summary>
        /// <param name="index">Item to retrieve information on.</param>
        /// <returns>The return value of the specified item.</returns>
        public int GetDeviceCaps(DeviceCapsItem index)
        {
            return ISession.CeGetDesktopDeviceCaps((int)index);
        }

        /// <summary>
        /// Retrieves the amount of space on a disk volume on a remote device.
        /// </summary>
        /// <param name="drivePath">String that specifies a directory on a disk.</param>
        /// <returns><see cref="DriveInfo"/> structure with information about specified disk.</returns>
        public DriveInfo GetDriveInfo(string drivePath)
        {
            DriveInfo di = new DriveInfo();
            if (0 == ISession.CeGetDiskFreeSpaceEx(drivePath, ref di.AvailableFreeSpace, ref di.TotalSize, ref di.TotalFreeSpace))
                ThrowRAPIException();
            return di;
        }

        /// <summary>
        /// Retrieves the path to a special shell folder on a remote device.
        /// </summary>
        /// <param name="folder">SpecialFolder enumeration.</param>
        /// <returns>Path of special folder on device.</returns>
        public string GetFolderPath(SpecialFolder folder)
        {
            var sb = new StringBuilder(MAX_PATH);
            if (0 == ISession.CeGetSpecialFolderPath((int)folder, MAX_PATH, sb))
                ThrowRAPIException();
            return sb.ToString();
        }

        /// <summary>
        /// Gets the shortcut target.
        /// </summary>
        /// <param name="shortcutFileName">Name of the shortcut file.</param>
        /// <returns>A <see cref="String"/> containing the path of the shortcut target.</returns>
        public string GetShortcutTarget(string shortcutFileName)
        {
            var sb = new StringBuilder(MAX_PATH);
            if (0 == ISession.CeSHGetShortcutTarget(shortcutFileName, sb, MAX_PATH))
                ThrowRAPIException();
            return sb.ToString();
        }

        /// <summary>
        /// This method retrieves the dimensions of display elements and system configuration settings of a remote device. All dimensions are in pixels.
        /// </summary>
        /// <param name="index">Item to retrieve information on.</param>
        /// <returns>The return value of the specified item.</returns>
        public int GetSystemMetrics(SystemMetricsItem index)
        {
            return ISession.CeGetSystemMetrics((int)index);
        }

        /// <summary>
        /// Gets the path to the directory designated for temporary files on a remote device.
        /// </summary>
        /// <returns>Temporary path.</returns>
        public string GetTempPath()
        {
            var sb = new StringBuilder(MAX_PATH);
            if (0 == ISession.CeGetTempPath(MAX_PATH, sb))
                ThrowRAPIException();
            return sb.ToString();
        }

        /// <summary>
        /// Starts the synchronization process with the device.
        /// </summary>
        public void StartSync()
        {
            ISession.CeSyncStart(null);
        }

        /// <summary>
        /// Stops the synchronization process with the device.
        /// </summary>
        public void StopSync()
        {
            ISession.CeSyncStop();
        }

        internal static RAPI_DEVICEINFO GetDeviceInfo(IRAPIDevice iDevice)
        {
            RAPI_DEVICEINFO di = new RAPI_DEVICEINFO();
            if (iDevice != null)
                iDevice.GetDeviceInfo(ref di);
            return di;
        }

        internal static object MarshalArrayToStruct(byte[] bits, Type objType)
        {
            IntPtr ptr = Marshal.AllocHGlobal(bits.Length);
            Marshal.Copy(bits, 0, ptr, bits.Length);
            object ret = Marshal.PtrToStructure(ptr, objType);
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        internal void Reconfigure(ref RAPI_DEVICEINFO di)
        {
            IDevice = null;
            cache = di;
        }

        internal void ResetSession()
        {
            if (iSession != null)
                try { iSession.CeRapiUninit(); } catch { }
            iSession = null;
            sessionFailure = false;
        }

        internal void ThrowRAPIException()
        {
            throw new RapiException(ISession.CeGetLastError());
        }

        /// <summary>Information about a space on a disk.</summary>
        public struct DriveInfo
        {
            /// <summary>The total number of free bytes on a disk that are available to the user.</summary>
            public ulong AvailableFreeSpace;

            /// <summary>The total number of bytes on a disk that are available to the user.</summary>
            public ulong TotalFreeSpace;

            /// <summary>The total number of free bytes on a disk</summary>
            public ulong TotalSize;
        }

        /// <summary>
        /// Represents a key-level node in the remote device's registry. This class is a registry encapsulation.
        /// </summary>
        public sealed class DeviceRegistryKey : IDisposable
        {
            private const int ERROR_FILE_NOT_FOUND = 2;
            private const int ERROR_MORE_DATA = 234;
            private const int ERROR_NO_MORE_ITEMS = 259;
            private const int ERROR_SUCCESS = 0;

            private uint hKey = 0;
            private readonly IRAPISession sess;

            internal DeviceRegistryKey(IRAPISession session)
            {
                sess = session;
            }

            internal DeviceRegistryKey(IRAPISession session, uint handle, string keyName)
            {
                sess = session;
                hKey = handle;
                Name = keyName;
            }

            /// <summary>
            /// Gets the name of the registry key.
            /// </summary>
            /// <value>The name of the registry key.</value>
            public string Name { get; private set; }

            /// <summary>
            /// Closes the key and flushes it to disk if its contents have been modified.
            /// </summary>
            public void Close()
            {
                Dispose();
            }

            /// <summary>
            /// Creates a new subkey or opens an existing subkey.
            /// </summary>
            /// <param name="subkey">Name of key to create.</param>
            /// <returns>A <see cref="DeviceRegistryKey"/> object that represents the newly created subkey, or <c>null</c> if the operation failed.</returns>
            public DeviceRegistryKey CreateSubKey(string subkey)
            {
                EnsureNotDisposed();
                uint hNewKey = 0, disp = 0;
                int ret = sess.CeRegCreateKeyEx(hKey, subkey, 0, string.Empty, 0, 0, IntPtr.Zero, ref hNewKey, ref disp);
                if (ret != ERROR_SUCCESS)
                    throw new RapiException(ret);
                return new DeviceRegistryKey(sess, hNewKey, Name + "\\" + subkey);
            }

            /// <summary>
            /// Deletes the specified subkey.
            /// </summary>
            /// <param name="subkey">The name of the subkey to delete.</param>
            /// <remarks>This method will fail if named subkey has children.</remarks>
            public void DeleteSubKey(string subkey)
            {
                DeleteSubKey(subkey, false);
            }

            /// <summary>
            /// Deletes the specified subkey.
            /// </summary>
            /// <param name="subkey">The name of the subkey to delete.</param>
            /// <param name="recursive">if set to <c>true</c> delete all subkeys under this subkey.</param>
            /// <exception cref="Rapi2.RapiException"></exception>
            public void DeleteSubKey(string subkey, bool recursive)
            {
                EnsureNotDisposed();
                if (recursive)
                {
                    using DeviceRegistryKey subKey = OpenSubKey(subkey);
                    foreach (var childKey in subKey.GetSubKeyNames())
                        subKey.DeleteSubKey(childKey, true);
                }
                int ret = sess.CeRegDeleteKey(hKey, subkey);
                if (ret != ERROR_SUCCESS && ret != ERROR_FILE_NOT_FOUND)
                    throw new RapiException(ret);
            }

            /// <summary>
            /// Deletes the specified value from this key.
            /// </summary>
            /// <param name="name">The name of the value to delete.</param>
            public void DeleteValue(string name)
            {
                EnsureNotDisposed();
                int ret = sess.CeRegDeleteValue(hKey, name);
                if (ret != ERROR_SUCCESS && ret != ERROR_FILE_NOT_FOUND)
                    throw new RapiException(ret);
            }

            /// <summary>
            /// Performs a close on the current key.
            /// </summary>
            public void Dispose()
            {
                sess.CeRegCloseKey(hKey);
                hKey = 0;
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Retrieves an array of strings that contains all the subkey names.
            /// </summary>
            /// <returns>An array of strings that contains the names of the subkeys for the current key.</returns>
            public string[] GetSubKeyNames()
            {
                uint idx = 0;
                StringBuilder sb = new StringBuilder(MAX_PATH);
                StringBuilder sbNull = new StringBuilder(MAX_PATH);
                uint cbName = MAX_PATH;
                uint cbClass = MAX_PATH;
                int ret;
                var list = new List<string>();
                do
                {
                    cbName = (uint)sb.Capacity;
                    ret = sess.CeRegEnumKeyEx(hKey, idx, sb, ref cbName, 0, sbNull, ref cbClass, IntPtr.Zero);
                    if (ret == ERROR_MORE_DATA)
                    {
                        sb.Capacity = (int)cbName + 1;
                        ret = sess.CeRegEnumKeyEx(hKey, idx, sb, ref cbName, 0, sbNull, ref cbClass, IntPtr.Zero);
                    }
                    if (ret == ERROR_SUCCESS)
                        list.Add(sb.ToString());
                    if (ret == ERROR_NO_MORE_ITEMS)
                        break;
                    idx++;
                } while (ret == ERROR_SUCCESS);

                if (ret != ERROR_NO_MORE_ITEMS)
                    throw new RapiException(ret);

                return list.ToArray();
            }

            /// <summary>
            /// Retrieves the value associated with the specified name. If the name is not found, returns the default value that you provide.
            /// </summary>
            /// <param name="name">The name of the value to retrieve.</param>
            /// <param name="defaultValue">The value to return if name does not exist.</param>
            /// <returns>The value associated with name, with any embedded environment variables left unexpanded, or defaultValue if name is not found.</returns>
            public object GetValue(string name, object defaultValue)
            {
                EnsureNotDisposed();
                int cbData = 0;
                int ret = sess.CeRegQueryValueEx(hKey, name, IntPtr.Zero, out _, IntPtr.Zero, ref cbData);
                if (ret != ERROR_SUCCESS)
                    throw new RapiException(ret);
                HGlobalSafeHandle data = new HGlobalSafeHandle(cbData);
                ret = sess.CeRegQueryValueEx(hKey, name, IntPtr.Zero, out int lpType, data, ref cbData);
                if (ret != 0)
                    throw new RapiException(ret);
                if (data == IntPtr.Zero)
                    return defaultValue;
                byte[] buffer = new byte[cbData];
                Marshal.Copy(data, buffer, 0, cbData);
                switch (lpType)
                {
                    case (int)RegistryValueKind.ExpandString:
                    case (int)RegistryValueKind.String:
                        return Encoding.Unicode.GetString(buffer);
                    case (int)RegistryValueKind.DWord:
                        return BitConverter.ToInt32(buffer, 0);
                    case (int)RegistryValueKind.QWord:
                        return BitConverter.ToInt64(buffer, 0);
                    case (int)RegistryValueKind.Binary:
                        return buffer;
                    case (int)RegistryValueKind.MultiString:
                        return Encoding.Unicode.GetString(buffer).TrimEnd('\0').Split('\0');
                    default:
                        return defaultValue;
                }
            }

            /// <summary>
            /// Gets the type of value in a registry value.
            /// </summary>
            /// <param name="name">The name of the value whose registry data type is to be retrieved.</param>
            /// <returns>A RegistryValueKind value representing the registry data type of the value associated with name.</returns>
            public RegistryValueKind GetValueKind(string name)
            {
                EnsureNotDisposed();
                int lpcbData = 0;
                int ret = sess.CeRegQueryValueEx(hKey, name, IntPtr.Zero, out int lpType, IntPtr.Zero, ref lpcbData);
                if (ret != ERROR_SUCCESS)
                    throw new RapiException(ret);
                if (!Enum.IsDefined(typeof(RegistryValueKind), lpType))
                    return RegistryValueKind.Unknown;
                return (RegistryValueKind)lpType;
            }

            /// <summary>
            /// Retrieves an array of strings that contains all the value names associated with this key.
            /// </summary>
            /// <returns>An array of strings that contains the value names for the current key.</returns>
            public string[] GetValueNames()
            {
                StringBuilder sb = new StringBuilder(MAX_PATH);
                uint idx = 0, cbName = MAX_PATH, cbData = 0;
                var list = new List<string>();
                int ret;
                do
                {
                    cbName = (uint)sb.Capacity;
                    ret = sess.CeRegEnumValue(hKey, idx, sb, ref cbName, 0, out _, IntPtr.Zero, ref cbData);
                    if (ret == ERROR_MORE_DATA)
                    {
                        sb.Capacity = (int)cbName + 1;
                        ret = sess.CeRegEnumValue(hKey, idx, sb, ref cbName, 0, out _, IntPtr.Zero, ref cbData);
                    }
                    if (ret == ERROR_SUCCESS)
                        list.Add(sb.ToString());
                    if (ret == ERROR_NO_MORE_ITEMS)
                        break;
                    idx++;
                } while (ret == ERROR_SUCCESS);

                if (ret != ERROR_NO_MORE_ITEMS)
                    throw new RapiException(ret);

                return list.ToArray();
            }

            /// <summary>
            /// Retrieves a subkey as read-only.
            /// </summary>
            /// <param name="name">The name or path of the subkey to open read-only.</param>
            /// <returns>The subkey requested, or null if the operation failed.</returns>
            public DeviceRegistryKey OpenSubKey(string name)
            {
                EnsureNotDisposed();
                uint hNewKey = 0;
                int ret = sess.CeRegOpenKeyEx(hKey, name, 0, 0, ref hNewKey);
                if (ret != ERROR_SUCCESS)
                    throw new RapiException(ret);
                return new DeviceRegistryKey(sess, hNewKey, Name + "\\" + name);
            }

            /// <summary>
            /// Sets the specified name/value pair.
            /// </summary>
            /// <param name="name">The name of the value to store.</param>
            /// <param name="value">The data to be stored.</param>
            public void SetValue(string name, object value)
            {
                var valueKind = RegistryValueKind.Unknown;
                if (value is int)
                    valueKind = RegistryValueKind.DWord;
                else if (value is long)
                    valueKind = RegistryValueKind.QWord;
                else if (value is string)
                    valueKind = RegistryValueKind.String;
                else if (value is byte[])
                    valueKind = RegistryValueKind.Binary;
                else if (value is string[])
                    valueKind = RegistryValueKind.MultiString;
                if (valueKind == RegistryValueKind.Unknown)
                    throw new ArgumentException("value must be of a known type");
                SetValue(name, value, valueKind);
            }

            /// <summary>
            /// Sets the value of a name/value pair in the registry key, using the specified registry data type.
            /// </summary>
            /// <param name="name">The name of the value to store.</param>
            /// <param name="value">The data to be stored.</param>
            /// <param name="valueKind">The registry data type to use when storing the data.</param>
            public void SetValue(string name, object value, RegistryValueKind valueKind)
            {
                EnsureNotDisposed();
                HGlobalSafeHandle data;
                int cbData;
                switch (valueKind)
                {
                    case RegistryValueKind.Binary:
                        cbData = ((byte[])value).Length;
                        data = new HGlobalSafeHandle((byte[])value);
                        break;
                    case RegistryValueKind.DWord:
                        cbData = sizeof(int);
                        data = new HGlobalSafeHandle(BitConverter.GetBytes(Convert.ToInt32(value)));
                        break;
                    case RegistryValueKind.String:
                    case RegistryValueKind.ExpandString:
                    case RegistryValueKind.MultiString:
                        string str = (valueKind == RegistryValueKind.MultiString ? string.Join("\0", (string[])value) : value.ToString()) + '\0';
                        byte[] bytes = Encoding.Unicode.GetBytes(str);
                        cbData = bytes.Length;
                        data = new HGlobalSafeHandle(bytes);
                        break;
                    case RegistryValueKind.QWord:
                        cbData = sizeof(long);
                        data = new HGlobalSafeHandle(BitConverter.GetBytes(Convert.ToInt64(value)));
                        break;
                    case RegistryValueKind.Unknown:
                    default:
                        throw new InvalidOperationException();
                }
                int ret = sess.CeRegSetValueEx(hKey, name, 0, (int)valueKind, data, cbData);
                if (ret != ERROR_SUCCESS)
                    throw new RapiException(ret);
            }

            /// <summary>
            /// Retrieves a string representation of this key.
            /// </summary>
            /// <returns>Key name.</returns>
            public override string ToString()
            {
                return Name;
            }

            private void EnsureNotDisposed()
            {
                if (hKey == 0)
                    throw new ObjectDisposedException("DeviceRegistryKey");
            }
        }

        /// <summary>
        /// Enumerates all connected devices. Access through <see cref="RemoteDeviceManager.Devices"/>.
        /// </summary>
        public class RemoteDatabaseList : IEnumerable<RemoteDatabase>, IDisposable
        {
            private readonly uint dbType;
            private IRAPISession sess;

            internal RemoteDatabaseList(IRAPISession session, uint dbType)
            {
                sess = session;
                this.dbType = dbType;
            }

            /// <summary>
            /// Cleans up all internal references.
            /// </summary>
            public void Dispose()
            {
                sess = null;
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Returns the strongly typed enumerator.
            /// </summary>
            /// <returns>Enumerator</returns>
            public IEnumerator<RemoteDatabase> GetEnumerator()
            {
                return new RemoteDatabaseEnum(sess, dbType);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Internal enumerator for databases on a remote device.
            /// </summary>
            public class RemoteDatabaseEnum : IEnumerator<RemoteDatabase>, IDisposable
            {
                private RemoteDatabase current = null;
                private readonly uint dbType;
                private DeviceHandle handle;
                private readonly IRAPISession sess;

                internal RemoteDatabaseEnum(IRAPISession session, uint dbType)
                {
                    sess = session;
                    this.dbType = dbType;
                    Reset();
                }

                /// <summary>
                /// Gets the current item in the enumeration.
                /// </summary>
                public RemoteDatabase Current
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
                    if (handle != null)
                    {
                        handle.Dispose();
                        handle = null;
                    }
                }

                /// <summary>
                /// Moves to the next database.
                /// </summary>
                /// <returns>true if a database was found. Otherwise, false.</returns>
                public bool MoveNext()
                {
                    try
                    {
                        uint ret = sess.CeFindNextDatabase(handle);
                        if (ret == 0)
                            throw new RapiException(sess.CeGetLastError());
                        current = new RemoteDatabase(sess, ret);
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
                    Dispose();
                    handle = new DeviceHandle(sess, sess.CeFindFirstDatabase(dbType));
                    if (handle.IsInvalid)
                        throw new RapiException(sess.CeGetLastError());
                    current = null;
                }
            }
        }

        internal class DeviceFile : SafeHandle
        {
            internal string Name;

            private readonly IRAPISession sess;

            internal DeviceFile(IRAPISession sess, string fileName, uint dwDesiredAccess, uint dwShareMode, uint dwCreationDistribution, uint dwFlags)
                : base(InvalidHandleValue, true)
            {
                this.sess = sess;
                Name = fileName;
                SetHandle(sess.CeCreateFile(fileName, dwDesiredAccess, dwShareMode, IntPtr.Zero, dwCreationDistribution, dwFlags, IntPtr.Zero));
                if (IsInvalid)
                    throw new RapiException(sess.CeGetLastError());
            }

            internal DeviceFile(IRAPISession sess, string fileName)
                : this(sess, fileName, 0, 1, 3, 0x80)
            {
            }

            public override bool IsInvalid
            {
                get { return handle == IntPtr.Zero || handle == InvalidHandleValue; }
            }

            public ulong Size
            {
                get
                {
                    uint size = 0;
                    uint res = sess.CeGetFileSize(handle, ref size);
                    if (res == 0xFFFFFFFF)
                        throw new RapiException(sess.CeRapiGetError());
                    return res + ((ulong)size << 32);
                }
            }

            /// <summary>
            /// Allows to use DeviceFile as IntPtr
            /// </summary>
            public static implicit operator IntPtr(DeviceFile f)
            {
                return f.DangerousGetHandle();
            }

            public FileTimes GetFileTimes()
            {
                FileTimes ft = new FileTimes();
                if (0 == sess.CeGetFileTime(handle, ref ft.cft, ref ft.aft, ref ft.wft))
                    throw new RapiException(sess.CeGetLastError());
                return ft;
            }

            public int Read(byte[] array, int offset, int count)
            {
                byte[] buf = new byte[count];
                int read = 0;
                int res = sess.CeReadFile(handle, buf, (uint)count, ref read, IntPtr.Zero);
                if (0 == res)
                    throw new RapiException(sess.CeGetLastError());
                buf.CopyTo(array, offset);
                return read;
            }

            public long Seek(long offset, SeekOrigin origin)
            {
                int lowOffset = (int)offset;
                int highOffset = (int)(offset >> 32);
                uint res = sess.CeSetFilePointer(handle, lowOffset, ref highOffset, (uint)origin);
                if (0xFFFFFFFF == res)
                    throw new RapiException(sess.CeGetLastError());
                return res + ((long)highOffset << 32);
            }

            public void SetEndOfFile()
            {
                if (0 == sess.CeSetEndOfFile(handle))
                    throw new RapiException(sess.CeGetLastError());
            }

            public void SetFileTimes(DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
            {
                CFILETIME cft = creationTime.HasValue ? new CFILETIME(creationTime.Value) : null;
                CFILETIME aft = lastAccessTime.HasValue ? new CFILETIME(lastAccessTime.Value) : null;
                CFILETIME wft = lastWriteTime.HasValue ? new CFILETIME(lastWriteTime.Value) : null;
                if (0 == sess.CeSetFileTime(handle, cft, aft, wft))
                    throw new RapiException(sess.CeGetLastError());
            }

            public int Write(byte[] array, int offset, int count)
            {
                int ret = 0;
                byte[] buf;
                if (offset == 0)
                    buf = array;
                else
                {
                    buf = new byte[count];
                    array.CopyTo(buf, offset);
                }
                if (0 == sess.CeWriteFile(base.handle, buf, count, ref ret, IntPtr.Zero))
                    throw new RapiException(sess.CeGetLastError());
                return ret;
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    if (0 == sess.CeCloseHandle(handle))
                        return false;
                }
                return true;
            }

            internal class FileTimes
            {
                public System.Runtime.InteropServices.ComTypes.FILETIME cft, aft, wft;

                public FileTimes()
                {
                }

                public DateTime CreationTime
                {
                    get { return cft.ToDateTime(); }
                    set { cft = value.ToFILETIME(); }
                }

                public DateTime LastAccessTime
                {
                    get { return aft.ToDateTime(); }
                    set { aft = value.ToFILETIME(); }
                }

                public DateTime LastWriteTime
                {
                    get { return wft.ToDateTime(); }
                    set { wft = value.ToFILETIME(); }
                }
            }
        }

        internal class DeviceHandle : SafeHandle
        {
            private readonly IRAPISession sess;

            public DeviceHandle(IRAPISession session, IntPtr handle)
                : base(InvalidHandleValue, true)
            {
                sess = session;
                SetHandle(handle);
            }

            public override bool IsInvalid
            {
                get { return handle == IntPtr.Zero || handle == InvalidHandleValue; }
            }

            /// <summary>
            /// Allows to use DeviceHandle as IntPtr
            /// </summary>
            public static implicit operator IntPtr(DeviceHandle f)
            {
                return f.DangerousGetHandle();
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    if (0 == sess.CeCloseHandle(base.handle))
                        return false;
                }
                return true;
            }
        }

        private class RAPIConnectionInfo
        {
            public IPEndPoint addr, host;
            public ConnectionType connType;

            public RAPIConnectionInfo(ref RAPI_CONNECTIONINFO ci)
            {
                connType = ci.connectionType;
                addr = ci.Ipaddr;
                host = ci.HostIpaddr;
            }
        }
    }
}