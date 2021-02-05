namespace Rapi2
{
    /// <summary>
    /// Items available in a call to RemoteDevice.GetDeviceCaps
    /// </summary>
    public enum DeviceCapsItem
    {
        /// <summary>Device driver version</summary>
        DRIVERVERSION = 0,
        /// <summary>Device classification</summary>
        TECHNOLOGY = 2,
        /// <summary>Horizontal size in millimeters</summary>
        HORZSIZE = 4,
        /// <summary>Vertical size in millimeters</summary>
        VERTSIZE = 6,
        /// <summary>Horizontal width in pixels</summary>
        HORZRES = 8,
        /// <summary>Vertical height in pixels</summary>
        VERTRES = 10,
        /// <summary>Number of bits per pixel</summary>
        BITSPIXEL = 12,
        /// <summary>Number of planes</summary>
        PLANES = 14,
        /// <summary>Number of brushes the device has</summary>
        NUMBRUSHES = 16,
        /// <summary>Number of pens the device has</summary>
        NUMPENS = 18,
        /// <summary>Number of markers the device has</summary>
        NUMMARKERS = 20,
        /// <summary>Number of fonts the device has</summary>
        NUMFONTS = 22,
        /// <summary>Number of colors the device supports</summary>
        NUMCOLORS = 24,
        /// <summary>Size required for device descriptor</summary>
        PDEVICESIZE = 26,
        /// <summary>Curve capabilities</summary>
        CURVECAPS = 28,
        /// <summary>Line capabilities</summary>
        LINECAPS = 30,
        /// <summary>Polygonal capabilities</summary>
        POLYGONALCAPS = 32,
        /// <summary>Text capabilities</summary>
        TEXTCAPS = 34,
        /// <summary>Clipping capabilities</summary>
        CLIPCAPS = 36,
        /// <summary>Bitblt capabilities</summary>
        RASTERCAPS = 38,
        /// <summary>Length of the X leg</summary>
        ASPECTX = 40,
        /// <summary>Length of the Y leg</summary>
        ASPECTY = 42,
        /// <summary>Length of the hypotenuse</summary>
        ASPECTXY = 44,
        /// <summary>Physical Width in device units</summary>
        PHYSICALWIDTH = 110,
        /// <summary>Physical Height in device units</summary>
        PHYSICALHEIGHT = 111,
        /// <summary>Physical Printable Area x margin</summary>
        PHYSICALOFFSETX = 112,
        /// <summary>Physical Printable Area y margin</summary>
        PHYSICALOFFSETY = 113,
        /// <summary>Shading and blending caps</summary>
        SHADEBLENDCAPS = 120,
    }
}