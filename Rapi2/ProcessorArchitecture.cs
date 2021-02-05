namespace Rapi2
{
    /// <summary>
    /// Processor Architecture values (GetSystemInfo)
    /// </summary>
    public enum ProcessorArchitecture : short
    {
        /// <summary>
        /// Intel
        /// </summary>
        Intel = 0,
        /// <summary>
        /// MIPS
        /// </summary>
        MIPS = 1,
        /// <summary>
        /// Alpha
        /// </summary>
        Alpha = 2,
        /// <summary>
        /// PowerPC
        /// </summary>
        PPC = 3,
        /// <summary>
        /// Hitachi SHx
        /// </summary>
        SHX = 4,
        /// <summary>
        /// ARM
        /// </summary>
        ARM = 5,
        /// <summary>
        /// IA64
        /// </summary>
        IA64 = 6,
        /// <summary>
        /// Alpha 64
        /// </summary>
        Alpha64 = 7,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = -1
    }
}