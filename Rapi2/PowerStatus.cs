namespace Rapi2
{
#pragma warning disable CS0649

    /// <summary>
    /// Structure for power information of mobile device
    /// </summary>
    public struct PowerStatus
    {
        /// <summary>
        /// AC Power status
        /// </summary>
        public byte ACLineStatus;
        /// <summary>
        /// Battery flag
        /// </summary>
        public byte BatteryFlag;
        /// <summary>
        /// Remaining battery life
        /// </summary>
        public byte BatteryLifePercent;
        internal byte Reserved1;
        /// <summary>
        /// Total battery life
        /// </summary>
        public int BatteryLifeTime;
        /// <summary>
        /// Battery life remaining
        /// </summary>
        public int BatteryFullLifeTime;
        internal byte Reserved2;
        /// <summary>
        /// Backup battery present
        /// </summary>
        public byte BackupBatteryFlag;
        /// <summary>
        /// Life remaining
        /// </summary>
        public byte BackupBatteryLifePercent;
        internal byte Reserved3;
        /// <summary>
        /// Life remaining
        /// </summary>
        public int BackupBatteryLifeTime;
        /// <summary>
        /// Total life when fully charged
        /// </summary>
        public int BackupBatteryFullLifeTime;
    }
#pragma warning restore CS0649
}