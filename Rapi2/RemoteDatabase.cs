using Rapi2.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Rapi2
{
    /// <summary>
    /// Represents a database on a device connected through RAPI2 (ActiveSync)
    /// </summary>
    public class RemoteDatabase : IDisposable
    {
        private const int CEDB_AUTOINCREMENT = 1;

        private readonly RemoteDevice.DeviceHandle handle;
        private readonly IRAPISession sess;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDatabase"/> class and opens an existing database in the object store on a remote Microsoft® Windows® CE–based device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="name">Contains the name of the database to be opened.</param>
        /// <param name="autoIncr">If set to <c>true</c> current seek position is automatically incremented with each call to the ReadRecordProps method.</param>
        /// <param name="sortId">Property identifier of the primary key for the sort order in which the database is to be traversed. Subsequent calls to Seek assume this sort order. This parameter can be zero if the sort order is not important.</param>
        public RemoteDatabase(RemoteDevice device, string name, bool autoIncr, uint sortId)
        {
            sess = device.ISession;
            uint oid = 0;
            handle = new RemoteDevice.DeviceHandle(sess, sess.CeOpenDatabase(ref oid, name, sortId, autoIncr ? CEDB_AUTOINCREMENT : 0, IntPtr.Zero));
            if (handle.IsInvalid)
                device.ThrowRAPIException();
            ObjectId = oid;
            CurrentRecordId = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDatabase"/> class and opens an existing database in the object store on a remote Microsoft® Windows® CE–based device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="objId">The object identifier of the database to be opened.</param>
        /// <param name="autoIncr">If set to <c>true</c> current seek position is automatically incremented with each call to the ReadRecordProps method.</param>
        /// <param name="sortId">Property identifier of the primary key for the sort order in which the database is to be traversed. Subsequent calls to Seek assume this sort order. This parameter can be zero if the sort order is not important.</param>
        public RemoteDatabase(RemoteDevice device, uint objId, bool autoIncr, uint sortId)
        {
            sess = device.ISession;
            handle = new RemoteDevice.DeviceHandle(sess, sess.CeOpenDatabase(ref objId, null, sortId, autoIncr ? CEDB_AUTOINCREMENT : 0, IntPtr.Zero));
            if (handle.IsInvalid)
                device.ThrowRAPIException();
            ObjectId = objId;
            CurrentRecordId = 0;
        }

        internal RemoteDatabase(IRAPISession s, uint objId)
        {
            sess = s;
            handle = new RemoteDevice.DeviceHandle(sess, sess.CeOpenDatabase(ref objId, null, 0, CEDB_AUTOINCREMENT, IntPtr.Zero));
            if (handle.IsInvalid)
                throw new RapiException(sess.CeGetLastError());
            ObjectId = objId;
            CurrentRecordId = 0;
        }

        /// <summary>
        /// Specifies the type of seek operation to perform on a <see cref="Seek"/> method call.
        /// </summary>
        public enum DatabaseSeekType : uint
        {
            /// <summary>
            /// Seeks until finding an object that has the specified object identifier. The dwValue parameter specifies the object identifier. This type of seek operation is very efficient.
            /// </summary>
            CEOID = 0x00000001,
            /// <summary>
            /// Seeks until finding the record at the specified position from the beginning of the database. The dwValue parameter specifies the number of records to seek from the beginning.
            /// </summary>
            Beginning = 0x00000002,
            /// <summary>
            /// Seeks backward for the specified number of records from the end of the database. The dwValue parameter specifies the number of records to seek from the end.
            /// </summary>
            End = 0x00000004,
            /// <summary>
            /// Seeks backward or forward from the current position of the seek pointer for the specified number of records. The dwValue parameter specifies the number of records to seek from the current position. The function seeks forward if dwValue is a positive value, or backward if it is negative.
            /// </summary>
            Current = 0x00000008,
            /// <summary>
            /// Starting from the current position, seeks backward toward the start of the sort. Always seeks backward, regardless of sort order. In ascending order, this finds the largest value smaller than the specified value; in descending order, this finds the smallest value larger than the specified value. If none of the previous records has a value that meets the search criteria, the seek pointer is left at the end of the database and the function returns 0. The dwValue parameter is a pointer to a CEPROPVAL structure.
            /// </summary>
            ValueSmaller = 0x00000010,
            /// <summary>
            /// Begins at the start of the sort and seeks forward until finding the first value that is equal to the specified value. Always seeks forward, regardless of sort order. If the seek operation fails, the seek pointer is left at the end of the database, and the function returns 0. The dwValue parameter is a pointer to a CEPROPVAL structure.
            /// </summary>
            ValueFirstEqual = 0x00000020,
            /// <summary>
            /// Starting from the current position, seeks backward toward the start of the sort. Always seeks forward, regardless of sort order. In ascending order this finds the smallest value greater than the specified value; in descending order this finds the largest value smaller than the specified value. If none of the following records has a value that meets the search criteria, the seek pointer is left at the end of the database and the function returns 0. The dwValue parameter is a pointer to a CEPROPVAL structure.
            /// </summary>
            ValueGreater = 0x00000040,
            /// <summary>
            /// Starting from the current seek position, seeks exactly one position forward in the sorted order and checks if the next record is equal in value to the specified value. If so, returns the object identifier of this next record; otherwise, returns 0 and leaves the seek pointer at the end of the database. This operation can be used in conjunction with the CEDB_SEEK_VALUEFIRSTEQUAL operation to enumerate all records with an equal value. The dwValue parameter is a pointer to a CEPROPVAL structure.
            /// </summary>
            ValueNextEqual = 0x00000080
        }

        [Flags]
        private enum DBaseInfoFlags
        {
            CEDB_VALIDNAME = 0x0001,
            CEDB_VALIDTYPE = 0x0002,
            CEDB_VALIDSORTSPEC = 0x0004,
            CEDB_VALIDMODTIME = 0x0008,
            CEDB_VALIDDBFLAGS = 0x0010
        }

        /// <summary>
        /// Gets the current record id.
        /// </summary>
        /// <value>The current record id.</value>
        public uint CurrentRecordId
        {
            get; private set;
        }

        /// <summary>
        /// Gets the date and time of the last time the database was written to.
        /// </summary>
        /// <value>The the date and time of the last time the database was written to.</value>
        public DateTime LastWriteTime
        {
            get { return GetDBInfo().ftLastModified.ToDateTime(); }
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The database name.</value>
        public string Name
        {
            get { return GetDBInfo().szDbaseName; }
        }

        /// <summary>
        /// Gets the object identifier of the database.
        /// </summary>
        /// <value>The object id.</value>
        public uint ObjectId
        {
            get; private set;
        }

        /// <summary>
        /// Gets the number of the records in the database.
        /// </summary>
        /// <value>The number of the records in the database.</value>
        public uint RecordCount
        {
            get { return GetDBInfo().wNumRecords; }
        }

        /// <summary>
        /// Gets the size of the database.
        /// </summary>
        /// <value>The database size in bytes.</value>
        public uint Size
        {
            get { return GetDBInfo().dwSize; }
        }

        /// <summary>
        /// Gets or sets the values of a specified record.
        /// </summary>
        /// <value>Array of <see cref="PropertyValue"/> structures representing the ordered properties of the record.</value>
        public PropertyValue[] this[uint record]
        {
            get
            {
                Seek(DatabaseSeekType.CEOID, record);
                return ReadRecordProps(null);
            }
            set
            {
                WriteRecordProps(record, value);
            }
        }

        /// <summary>
        /// Gets the value of a property for the specified record.
        /// </summary>
        /// <value>Value of the specified property.</value>
        public object this[int record, uint propId]
        {
            get
            {
                Seek(DatabaseSeekType.CEOID, (uint)record);
                return ReadRecordProps(new uint[] { propId })[0].Value;
            }
        }

        /// <summary>
        /// Creates the specified database on the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="name"><para>Specifies the name for the new database. The name can have up to 31 characters. If the name is too long, it is truncated and the databases is created. Do not use an empty string as the database name.</para>
        /// <para>Note: Do not create a database name that ends with a backslash character "\".</para></param>
        /// <param name="identifier">Specifies the type identifier for the database. This is an application-defined value that can be used for any application-defined purpose. For example, an application can use the type identifier to distinguish address book data from to-do list data or use the identifier during a database enumeration sequence. The type identifier is not meant to be a unique identifier for the database. The system does not use this value.</param>
        /// <param name="sortDesc">An array of sort order descriptions.</param>
        /// <returns>The object identifier of the newly created database.</returns>
        public static uint Create(RemoteDevice device, string name, uint identifier, params SortOrderDescriptor[] sortDesc)
        {
            if (sortDesc.Length > 4)
                throw new ArgumentOutOfRangeException();
            uint id = device.ISession.CeCreateDatabase(name, identifier, (ushort)sortDesc.Length, sortDesc);
            if (id == 0)
                throw new RapiException(device.ISession.CeGetLastError());
            return id;
        }

        /// <summary>
        /// Removes a database from the object store on a remote Microsoft® Windows® CE–based device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="objId">Object identifier of the database to be deleted.</param>
        /// <returns></returns>
        public static bool Delete(RemoteDevice device, uint objId)
        {
            return device.ISession.CeDeleteDatabase(objId) == 1;
        }

        /// <summary>
        /// Deletes a record from a database on a remote Microsoft® Windows® CE–based device.
        /// </summary>
        /// <param name="recordId">Object identifier of the record to be deleted. This identifier can be obtained by calling the <see cref="ReadRecordProps"/> method.</param>
        public void DeleteRecord(uint recordId)
        {
            if (sess.CeDeleteRecord(handle, recordId) == 0)
                throw new RapiException(sess.CeGetLastError());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (handle != null) handle.Dispose();
        }

        /// <summary>
        /// Reads properties from the current record of a database on a remote Microsoft® Windows® CE–based device.
        /// </summary>
        /// <param name="propIds">An array specifying the identifiers of the properties to read.</param>
        /// <returns>An array of <see cref="PropertyValue"/> structures with the values of the specified properties.</returns>
        public PropertyValue[] ReadRecordProps(uint[] propIds)
        {
            const uint CEDB_ALLOWREALLOC = 1;
            ushort cProps = 0;
            IntPtr retBuf = IntPtr.Zero;
            int cBuf = 0;
            uint ret = sess.CeReadRecordProps(handle, CEDB_ALLOWREALLOC, ref cProps, propIds, ref retBuf, ref cBuf);
            if (ret == 0)
                throw new RapiException(sess.CeGetLastError());
            CurrentRecordId = ret;
            PropertyValue[] vals = new PropertyValue[cProps];
            Type t = typeof(PropertyValue);
            for (int i = 0; i < cProps; i++)
            {
                vals[i] = (PropertyValue)Marshal.PtrToStructure(new IntPtr(retBuf.ToInt32() + (Marshal.SizeOf(t) * i)), t);
            }
            Marshal.FreeHGlobal(retBuf);
            return vals;
        }

        /// <summary>
        /// Seeks the specified seek type.
        /// </summary>
        /// <param name="seekType">Specifies the type of seek operation to perform. See <see cref="DatabaseSeekType"/> for more information.</param>
        /// <param name="value">The value corresponding to the <c>seekType</c>. See <see cref="DatabaseSeekType"/> for more information.</param>
        /// <returns>The current record id.</returns>
        public uint Seek(DatabaseSeekType seekType, uint value)
        {
            int idx = 0;
            uint ret = sess.CeSeekDatabase(handle, (uint)seekType, value, ref idx);
            if (ret == 0)
                throw new RapiException(sess.CeGetLastError());
            CurrentRecordId = ret;
            return ret;
        }

        /// <summary>
        /// Writes a set of properties to a single database record on a remote Microsoft® Windows® CE–based device.
        /// </summary>
        /// <param name="recId">The id of the record to read. If this parameter is zero, a new record is created and filled in with the specified properties.</param>
        /// <param name="props">an array of <see cref="PropertyValue"/> structures that specify the property values to be written to the specified record.</param>
        /// <returns>The current record id.</returns>
        public uint WriteRecordProps(uint recId, PropertyValue[] props)
        {
            uint ret = sess.CeWriteRecordProps(handle, recId, (ushort)props.Length, props);
            if (ret == 0)
                throw new RapiException(sess.CeGetLastError());
            return ret;
        }

        private CEDBASEINFO GetDBInfo()
        {
            CEOIDINFO oidi = new CEOIDINFO();
            if (0 != sess.CeOidGetInfo(ObjectId, ref oidi) && oidi.wObjType == 3 /*OBJTYPE_DATABASE*/)
                return (CEDBASEINFO)RemoteDevice.MarshalArrayToStruct(oidi.inf, typeof(CEDBASEINFO));
            return new CEDBASEINFO();
        }

        /// <summary>
        /// This structure contains a property value.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        public struct PropertyValue
        {
            internal int propid;
            internal ushort wLenData;
            internal ushort wFlags;
            internal CEVALUNION val;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyValue"/> struct.
            /// </summary>
            /// <param name="value">The value.</param>
            public PropertyValue(object value)
            {
                propid = 0; wLenData = wFlags = 0; val = new CEVALUNION();
                if (value != null)
                    this.Value = value;
            }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public object Value
            {
                get
                {
                    return propid switch
                    {
                        2 => val.iVal,
                        18 => val.uiVal,
                        3 => val.lVal,
                        19 => val.ulVal,
                        64 => DateTime.FromFileTimeUtc(val.filetime),
                        31 => Marshal.PtrToStringUni(val.lpwstr),
                        65 => ((CEBLOB)Marshal.PtrToStructure(val.blob, typeof(CEBLOB))).Data,
                        11 => val.boolVal != 0,
                        5 => val.dblVal,
                        _ => throw new InvalidDataException(),
                    };
                }

                set
                {
                    if (value is short shortVal)
                    {
                        propid = 2;
                        val.iVal = shortVal;
                    }
                    else if (value is ushort uShortVal)
                    {
                        propid = 18;
                        val.uiVal = uShortVal;
                    }
                    else if (value is int intVal)
                    {
                        propid = 3;
                        val.lVal = intVal;
                    }
                    else if (value is uint uIntVal)
                    {
                        propid = 19;
                        val.ulVal = uIntVal;
                    }
                    else if (value is System.Runtime.InteropServices.ComTypes.FILETIME filetimeVal)
                    {
                        propid = 64;
                        val.filetime = filetimeVal.ToDateTime().ToFileTimeUtc();
                    }
                    else if (value is DateTime dateTimeVal)
                    {
                        propid = 64;
                        val.filetime = dateTimeVal.ToFileTimeUtc();
                    }
                    else if (value is string stringVal)
                    {
                        propid = 31;
                        val.lpwstr = Marshal.StringToHGlobalAuto(stringVal);
                    }
                    else if (value is byte[] byteArrVal)
                    {
                        propid = 65;
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CEBLOB)));
                        CEBLOB cblob = new CEBLOB(byteArrVal);
                        Marshal.StructureToPtr(cblob, ptr, true);
                        val.blob = ptr;
                    }
                    else if (value is bool boolVal)
                    {
                        propid = 11;
                        val.boolVal = boolVal ? 1 : 0;
                    }
                    else if (value is double doubleVal)
                    {
                        propid = 5;
                        val.dblVal = doubleVal;
                    }
                    else
                        throw new ArgumentException();
                }
            }
        }
    }
}