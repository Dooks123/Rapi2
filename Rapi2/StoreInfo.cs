namespace Rapi2
{
    /// <summary>
    /// Describes the current status of the Object Store
    /// </summary>
    public struct StoreInfo
    {
        /// <summary>
        /// Size of the Object Store in Bytes
        /// </summary>
        public int StoreSize;
        /// <summary>
        /// Free space in the Object Store in Bytes
        /// </summary>
        public int FreeSize;
    }
}