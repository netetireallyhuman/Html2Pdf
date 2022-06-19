using System.Collections.Generic;

namespace H2P
{
    /// <summary>
    /// Interface for readers providing string-lists.
    /// </summary>
    /// <remarks>
    /// 03.06.2022 Erik Nagel: created.
    /// </remarks>
    public interface IRawLineReader
    {
        /// <summary>
        /// Reads all lines of a given line-container into a string-list.
        /// </summary>
        /// <returns>A string-list from a given line-container.</returns>
        List<string> ReadAllLines();
    }
}
