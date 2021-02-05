using System;

namespace Rapi2
{
    /// <summary>
    /// Methods to extend date classes.
    /// </summary>
    public static class ExetensionMethods
    {
        /// <summary>
        /// Converts a FILETIME to a DateTime
        /// </summary>
        /// <param name="ft">FILETIME to convert.</param>
        /// <returns>Equivalent DateTime.</returns>
        public static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME ft)
        {
            long hFT2 = (((long)ft.dwHighDateTime) << 32) + ft.dwLowDateTime;

            try
            {
                return DateTime.FromFileTimeUtc(hFT2);
            }
            catch (ArgumentOutOfRangeException)
            {
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Converts a DateTime to a FILETIME
        /// </summary>
        /// <param name="dt">DateTime to convert.</param>
        /// <returns>Equivalent FILETIME.</returns>
        public static System.Runtime.InteropServices.ComTypes.FILETIME ToFILETIME(this DateTime dt)
        {
            var ft = new System.Runtime.InteropServices.ComTypes.FILETIME();
            long hFT1 = dt.ToFileTimeUtc();
            ft.dwLowDateTime = (int)(hFT1 & 0xFFFFFFFF);
            ft.dwHighDateTime = (int)(hFT1 >> 32);
            return ft;
        }
    }
}