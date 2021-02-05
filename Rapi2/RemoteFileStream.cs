using System;
using System.IO;

namespace Rapi2
{
    /// <summary>
    /// Exposes a Stream around a remote file, supporting synchronous read and write operations. 
    /// </summary>
    public class RemoteFileStream : Stream
    {
        internal const int DefaultBufferSize = 0x80;

        private readonly long _appendStart;
        private byte[] _buffer;
        private int _bufferSize;
        private readonly bool _canRead;
        private readonly bool _canSeek;
        private readonly bool _canWrite;
        private ulong _pos;
        private int _readLen;
        private int _readPos;
        private int _writePos;
        private readonly RemoteDevice.DeviceFile f;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFileStream"/> class with the specified path and creation mode.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="path">A relative or absolute path for the file that the current <see cref="RemoteFileStream"/> object will encapsulate.</param>
        /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
        public RemoteFileStream(RemoteDevice device, string path, FileMode mode)
            : this(device, path, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.Read, FileAttributes.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFileStream"/> class with the specified path, creation mode, and read/write permission.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="path">A relative or absolute path for the file that the current <see cref="RemoteFileStream"/> object will encapsulate.</param>
        /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
        /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
        public RemoteFileStream(RemoteDevice device, string path, FileMode mode, FileAccess access)
            : this(device, path, mode, access, FileShare.Read, FileAttributes.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFileStream"/> class with the specified path, creation mode, read/write permission, and sharing permission.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="path">A relative or absolute path for the file that the current <see cref="RemoteFileStream"/> object will encapsulate.</param>
        /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
        /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
        /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
        public RemoteFileStream(RemoteDevice device, string path, FileMode mode, FileAccess access, FileShare share)
            : this(device, path, mode, access, share, FileAttributes.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFileStream"/> class with the specified path, creation mode, read/write permission, sharing permission, and attributes.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="path">A relative or absolute path for the file that the current <see cref="RemoteFileStream"/> object will encapsulate.</param>
        /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
        /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
        /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
        /// <param name="attributes">The <see cref="FileAttributes"/> to set on the new file.</param>
        public RemoteFileStream(RemoteDevice device, string path, FileMode mode, FileAccess access, FileShare share, FileAttributes attributes)
        {
            f = new RemoteDevice.DeviceFile(device.ISession, path, ((uint)access << 30), (uint)share, (uint)mode, (uint)attributes);
            _canRead = (access & FileAccess.Read) != 0;
            _canWrite = (access & FileAccess.Write) != 0;
            _canSeek = true;
            _pos = 0L;
            _bufferSize = DefaultBufferSize;
            _readPos = 0;
            _readLen = 0;
            _writePos = 0;
            if (mode == FileMode.Append)
                _appendStart = SeekCore(0L, SeekOrigin.End);
            else
                _appendStart = -1L;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the stream supports reading; <c>false</c> if the stream is closed or was opened with write-only permissions.</returns>
        public override bool CanRead
        {
            get { return _canRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the stream supports seeking; <c>false</c> if the stream is closed or if the <see cref="RemoteFileStream"/> was constructed from an operating-system handle such as a pipe or output to the console.</returns>
        public override bool CanSeek
        {
            get { return _canSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the stream supports writing; <c>false</c> if the stream is closed or was opened with read-only access.</returns>
        public override bool CanWrite
        {
            get { return _canWrite; }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value></value>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        public override long Length
        {
            get
            {
                if (!CanSeek)
                    throw new NotSupportedException();

                ulong l = f.Size;
                if ((_writePos > 0) && ((_pos + (ulong)_writePos) > l))
                {
                    l = (ulong)_writePos + _pos;
                }
                return (long)l;
            }
        }

        /// <summary>
        /// Gets the name of the file that was passed to the constructor.
        /// </summary>
        /// <value>The name of the file passed to the constructor.</value>
        public string Name { get { return f.Name; } }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value></value>
        /// <returns>The current position within the stream.</returns>
        public override long Position
        {
            get
            {
                if (!CanSeek)
                    throw new NotSupportedException();
                VerifyOSHandlePosition();
                return (long)(_pos + (ulong)(_readPos - _readLen + _writePos));
            }
            set
            {
                if (value < 0L)
                    throw new ArgumentOutOfRangeException();
                if (_writePos > 0)
                    FlushWrite();
                _readPos = 0;
                _readLen = 0;
                Seek(value, SeekOrigin.Begin);
            }
        }

        internal int BufferSize
        {
            get { return _bufferSize; }
            set { Flush(); _bufferSize = value; _buffer = null; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            if (_writePos > 0)
            {
                FlushWrite();
                f.SetEndOfFile();
            }
            else if (_readPos < _readLen && CanSeek)
            {
                FlushRead();
            }
        }

        /// <summary>
        /// Reads a block of bytes from the stream and writes the data in a given buffer.
        /// </summary>
        /// <param name="array">When this method returns, contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">The byte offset in <paramref name="array"/> at which the read bytes will be placed.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The total number of bytes read into the buffer. This might be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached.</returns>
        public override int Read(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentException();
            }
            bool flag = false;
            int num = _readLen - _readPos;
            if (num == 0)
            {
                if (!CanRead)
                {
                    throw new NotSupportedException();
                }
                if (_writePos > 0)
                {
                    FlushWrite();
                }
                if (count >= _bufferSize)
                {
                    return ReadCore(array, offset, count);
                }
                if (_buffer == null)
                {
                    _buffer = new byte[_bufferSize];
                }
                num = ReadCore(_buffer, 0, _bufferSize);
                if (num == 0)
                {
                    return 0;
                }
                flag = num < _bufferSize;
                _readPos = 0;
                _readLen = num;
            }
            if (num > count)
            {
                num = count;
            }
            Buffer.BlockCopy(_buffer, _readPos, array, offset, num);
            _readPos += num;
            if ((num < count) && !flag)
            {
                int num2 = ReadCore(array, offset + num, count - num);
                num += num2;
            }
            return num;
        }

        /// <summary>
        /// Sets the current position of this stream to the given value.
        /// </summary>
        /// <param name="offset">The point relative to <paramref name="origin"/> from which to begin seeking.</param>
        /// <param name="origin">A value of type <see cref="T:SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if ((origin < SeekOrigin.Begin) || (origin > SeekOrigin.End))
            {
                throw new ArgumentException();
            }
            if (!CanSeek)
            {
                throw new NotSupportedException();
            }
            if (_writePos > 0)
            {
                FlushWrite();
            }
            else if (origin == SeekOrigin.Current)
            {
                offset -= _readLen - _readPos;
            }
            VerifyOSHandlePosition();
            long num = (long)_pos + (_readPos - _readLen);
            long num2 = SeekCore(offset, origin);
            if ((_appendStart != -1L) && (num2 < _appendStart))
            {
                SeekCore(num, SeekOrigin.Begin);
                throw new IOException();
            }
            if (_readLen > 0)
            {
                if (num == num2)
                {
                    if (_readPos > 0)
                    {
                        Buffer.BlockCopy(_buffer, _readPos, _buffer, 0, _readLen - _readPos);
                        _readLen -= _readPos;
                        _readPos = 0;
                    }
                    if (_readLen > 0)
                    {
                        SeekCore(_readLen, SeekOrigin.Current);
                    }
                    return num2;
                }
                if (((num - _readPos) < num2) && (num2 < (num + _readLen - _readPos)))
                {
                    int num3 = (int)(num2 - num);
                    Buffer.BlockCopy(_buffer, _readPos + num3, _buffer, 0, _readLen - (_readPos + num3));
                    _readLen -= _readPos + num3;
                    _readPos = 0;
                    if (_readLen > 0)
                    {
                        SeekCore(_readLen, SeekOrigin.Current);
                    }
                    return num2;
                }
                _readPos = 0;
                _readLen = 0;
            }
            return num2;
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            if (value < 0L)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!CanSeek || !CanWrite)
            {
                throw new NotSupportedException();
            }
            if (_writePos > 0)
            {
                FlushWrite();
            }
            else if (_readPos < _readLen)
            {
                FlushRead();
            }
            if ((_appendStart != -1L) && (value < _appendStart))
            {
                throw new IOException();
            }
            SetLengthCore(value);
        }

        /// <summary>
        /// Writes a block of bytes to this stream using data from a buffer. 
        /// </summary>
        /// <param name="array">The buffer containing data to write to the stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="array"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentException();
            }
            if (_writePos == 0)
            {
                if (!CanWrite)
                {
                    throw new NotSupportedException();
                }
                if (_readPos < _readLen)
                {
                    FlushRead();
                }
                _readPos = 0;
                _readLen = 0;
            }
            if (_writePos > 0)
            {
                int num = _bufferSize - _writePos;
                if (num > 0)
                {
                    if (num > count)
                    {
                        num = count;
                    }
                    Buffer.BlockCopy(array, offset, _buffer, _writePos, num);
                    _writePos += num;
                    if (count == num)
                    {
                        return;
                    }
                    offset += num;
                    count -= num;
                }
                WriteCore(_buffer, 0, _writePos);
                _writePos = 0;
            }
            if (count >= _bufferSize)
            {
                WriteCore(array, offset, count);
            }
            else
            {
                if (_buffer == null)
                {
                    _buffer = new byte[_bufferSize];
                }
                Buffer.BlockCopy(array, offset, _buffer, _writePos, count);
                _writePos = count;
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="RemoteFileStream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (f != null && !f.IsClosed)
                f.Dispose();
        }

        private void FlushRead()
        {
            if ((_readPos - _readLen) != 0)
            {
                SeekCore(_readPos - _readLen, SeekOrigin.Current);
            }
            _readPos = 0;
            _readLen = 0;
        }

        private void FlushWrite()
        {
            WriteCore(_buffer, 0, _writePos);
            _writePos = 0;
        }

        private int ReadCore(byte[] buffer, int offset, int count)
        {
            int num2 = f.Read(buffer, offset, count);
            if (num2 == -1)
                throw new IOException();
            _pos += (ulong)num2;
            VerifyOSHandlePosition();
            return num2;
        }

        private long SeekCore(long offset, SeekOrigin origin)
        {
            long num = f.Seek(offset, origin);
            _pos = (ulong)num;
            return num;
        }

        private void SetLengthCore(long value)
        {
            if (value > 0x7fffffffL)
            {
                throw new ArgumentOutOfRangeException();
            }
            ulong offset = _pos;
            if (_pos != (ulong)value)
            {
                SeekCore(value, SeekOrigin.Begin);
            }
            f.SetEndOfFile();
            if ((long)offset != value)
            {
                if ((long)offset < value)
                {
                    SeekCore((long)offset, SeekOrigin.Begin);
                }
                else
                {
                    SeekCore(0L, SeekOrigin.End);
                }
            }
            VerifyOSHandlePosition();
        }

        private void VerifyOSHandlePosition()
        {
            if (CanSeek)
            {
                ulong num = _pos;
                if (SeekCore(0L, SeekOrigin.Current) != (long)num)
                {
                    _readPos = 0;
                    _readLen = 0;
                }
            }
        }

        private void WriteCore(byte[] buffer, int offset, int count)
        {
            if ((buffer.Length - offset) < count)
                throw new IndexOutOfRangeException();
            int num2 = f.Write(buffer, offset, count);
            if (num2 == -1)
                throw new IOException();
            _pos += (ulong)num2;
        }
    }
}