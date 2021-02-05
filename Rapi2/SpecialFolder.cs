namespace Rapi2
{
    /// <summary>
    /// Special folder defined on the device.
    /// </summary>
    public enum SpecialFolder
    {
        /// <summary>File system directory that serves as a common repository for application-specific data.</summary>
        ApplicationData = 0x001a,
        //RecylceBinFolder = 0x000a,
        /// <summary>File system directory used to physically store file objects on the desktop.</summary>
        Desktop = 0x0000,
        //DesktopDirectory = 0x0010,
        //MyComputer = 0x0011,
        /// <summary>File system directory that serves as a common repository for the user's favorite items.</summary>
        Favorites = 0x0006,
        /// <summary>Virtual folder containing fonts.</summary>
        Fonts = 0x0014,
        //MyMusic = 0x000d,
        /// <summary>The file system directory that serves as a common repository for image files. </summary>
        MyPictures = 0x0027,
        //MyVideo = 0x000e,
        //NetworkFolder = 0x0012,
        /// <summary>The file system directory used to physically store a user's common repository of documents.</summary>
        MyDocuments = 0x0005,
        /// <summary>Program files folder.</summary>
        ProgramFiles = 0x0026,
        //Programs = 0x0002,
        //Recent = 0x0008,
        /// <summary>File system directory that contains Start menu items.</summary>
        StartMenu = 0x000b,
        /// <summary>File system directory that corrsponds to the user's Startup program group. The system starts these programs when a device is powered on.</summary>
        Startup = 0x0007,
        /// <summary>Windows folder.</summary>
        Windows = 0x0024,
    }
}