namespace Rapi2
{
#pragma warning disable CS0649
    /// <summary>
    /// Data structure for GetSystemInfo
    /// </summary>
    public struct SystemInformation
    {
        /// <summary>
        /// Processor architecture
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture;
        internal ushort wReserved;
        /// <summary>
        /// Specifies the page size and the granularity of page protection and commitment.
        /// </summary>
        public uint PageSize;
        /// <summary>
        /// Pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs). 
        /// </summary>
        public uint MinimumApplicationAddress;
        /// <summary>
        /// Pointer to the highest memory address accessible to applications and DLLs.
        /// </summary>
        public uint MaximumApplicationAddress;
        /// <summary>
        /// Specifies a mask representing the set of processors configured into the system. Bit 0 is processor 0; bit 31 is processor 31. 
        /// </summary>
        public uint ActiveProcessorMask;
        /// <summary>
        /// Specifies the number of processors in the system.
        /// </summary>
        public uint NumberOfProcessors;
        /// <summary>
        /// Specifies the type of processor in the system.
        /// </summary>
        public ProcessorType dwProcessorType;
        /// <summary>
        /// Specifies the granularity with which virtual memory is allocated.
        /// </summary>
        public uint AllocationGranularity;
        /// <summary>
        /// Specifies the system’s architecture-dependent processor level.
        /// </summary>
        public ushort ProcessorLevel;
        /// <summary>
        /// Specifies an architecture-dependent processor revision.
        /// </summary>
        public ushort ProcessorRevision;
    }
#pragma warning restore CS0649
}