using System.Runtime.InteropServices;

namespace Rapi2
{
    /// <summary>
    /// This structure contains information about a sort order in a database.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SortOrderDescriptor
    {
        readonly uint propid;
        readonly uint dwFlags;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortOrderDescriptor"/> struct.
        /// </summary>
        /// <param name="pid">Identifier of the property to be sorted on. Sorts on binary properties are not allowed.</param>
        /// <param name="flgs">Sort flags. See <see cref="SortOrderFlags"/> for more detail.</param>
        public SortOrderDescriptor(uint pid, SortOrderFlags flgs)
        {
            propid = pid; dwFlags = (uint)flgs;
        }
    }
}