using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    /// <summary>
    /// Version info for the connected device
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CEOSVERSIONINFO
    {
        internal int dwOSVersionInfoSize;
        /// <summary>
        /// Major
        /// </summary>
        public int dwMajorVersion;
        /// <summary>
        /// Minor
        /// </summary>
        public int dwMinorVersion;
        /// <summary>
        /// Build
        /// </summary>
        public int dwBuildNumber;
        /// <summary>
        /// Platform type
        /// </summary>
        public int dwPlatformId;
        /// <summary>
        /// Null-terminated string that provides arbitrary additional information about the operating system.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
    }
}