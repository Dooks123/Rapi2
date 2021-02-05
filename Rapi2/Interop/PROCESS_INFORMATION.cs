using System;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;
		public IntPtr hThread;
		public int dwProcessID;
		public int dwThreadID;
	}
}