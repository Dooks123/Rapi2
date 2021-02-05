using System;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential)]
	internal class CFILETIME
	{
		public CFILETIME(DateTime dt)
		{
			long hFT1 = dt.ToFileTimeUtc();
			dwLowDateTime = (int)(hFT1 & 0xFFFFFFFF);
			dwHighDateTime = (int)(hFT1 >> 32);
		}
		public int dwLowDateTime;
		public int dwHighDateTime;
	}
}