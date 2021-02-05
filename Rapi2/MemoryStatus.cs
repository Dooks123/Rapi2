using System.Runtime.InteropServices;

namespace Rapi2
{
    /// <summary>
    /// Contains information about current memory availability for a remote device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryStatus
    {
        internal uint dwLength;
        /// <summary>
        /// Specifies a number between zero and 100 that gives a general idea of current memory use, in which zero indicates no memory use and 100 indicates full memory use.
        /// </summary>
        public uint MemoryLoad;
        /// <summary>
        /// Indicates the total number of bytes of physical memory.
        /// </summary>
        public uint TotalPhysical;
        /// <summary>
        /// Indicates the number of bytes of physical memory available.
        /// </summary>
        public uint AvailPhysical;
        /// <summary>
        /// Indicates the total number of bytes that can be stored in the paging file. This number does not represent the physical size of the paging file on disk.
        /// </summary>
        public uint TotalPageFile;
        /// <summary>
        /// Indicates the number of bytes available in the paging file.
        /// </summary>
        public uint AvailablePageFile;
        /// <summary>
        /// Indicates the total number of bytes that can be described in the user mode portion of the virtual address space of the calling process.
        /// </summary>
        public uint TotalVirtual;
        /// <summary>
        /// Indicates the number of bytes of unreserved and uncommitted memory in the user mode portion of the virtual address space of the calling process.
        /// </summary>
        public uint AvailableVirtual;
    }
}