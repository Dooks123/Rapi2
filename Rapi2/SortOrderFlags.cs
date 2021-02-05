using System;

namespace Rapi2
{
    /// <summary>
    /// Sort flags. The default is ascending order and case-sensitive. Records that do not contain the sort property are placed after all other records.
    /// </summary>
    [Flags]
    public enum SortOrderFlags : uint
    {
        /// <summary>
        /// Default sort order.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Causes the sort to be in descending order. By default, the sort is done in ascending order.
        /// </summary>
        Descending = 0x00000001,
        /// <summary>
        /// Causes thee sort operation to be case-insensitive. This value is valid only for strings.
        /// </summary>
        CaseInsensitive = 0x00000002,
        /// <summary>
        /// Causes the sort opperation to place records that do not contain the sort property before all the other records.
        /// </summary>
        UnknownFirst = 0x00000004,
        /// <summary>
        /// Specifies that this sort ignores nonspacing characters, such as accents, diacritics, and vowel marks. This value is valid only for strings.
        /// </summary>
        IgnorenOnSpace = 0x00000010,
        /// <summary>
        /// Causes the sort operation to not recognize symbol values. This value is valid only for strings.
        /// </summary>
        IgnoreSymbols = 0x00000020,
        /// <summary>
        /// The sort does not differentiate between Hiragana and Katakana characters. This value is valid only for strings.
        /// </summary>
        IgnoreKanaType = 0x00000040,
        /// <summary>
        /// Prevents the sort operation from differentiating between a single-byte character and the same character as a double-byte character. This value is valid only for strings.
        /// </summary>
        IgnoreWidth = 0x00000080,
        /// <summary>
        /// Requires the key to be unique across all records in the database. This constraint also requires the sort property to be present in all records.
        /// </summary>
        Unique = 0x00000200,
        /// <summary>
        /// Requires the sort property to be present in all records.
        /// </summary>
        NonNull = 0x00000400
    }
}