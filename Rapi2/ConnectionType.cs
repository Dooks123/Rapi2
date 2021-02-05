namespace Rapi2
{
    /// <summary>
    /// Mechanism used to connect to device.
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>A USB connection.</summary>
        USB = 0,
        /// <summary>An infrared connection.</summary>
        IR = 1,
        /// <summary>A serial connection.</summary>
        Serial = 2,
        /// <summary>A network connection.</summary>
        Network = 3,
    }
}