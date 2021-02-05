using System;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct CEBLOB
	{
		public CEBLOB(byte[] bytes)
		{
			size = bytes.Length;
			ptr = Marshal.AllocHGlobal(size);
			Marshal.Copy(bytes, 0, ptr, size);
		}

        readonly int size;
        readonly IntPtr ptr;
		public byte[] Data
		{
			get
			{
				byte[] bytes = new byte[size];
				Marshal.Copy(ptr, bytes, 0, size);
				return bytes;
			}
		}
	}
}