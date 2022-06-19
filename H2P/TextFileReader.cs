using System;
using System.Collections.Generic;
using System.IO;

namespace H2P
{
    /// <summary>
    /// Reads a textfile into a string-list.
    /// </summary>
    /// <remarks>
    /// 03.06.2022 Erik Nagel: created.
    /// </remarks>
    public class TextFileReader : IRawLineReader
    {
        ///<summary>Path to the Textfile to be read into a string-list.</summary>
        public string FileName { get; set; }

        /// <summary>
        /// Reads all lines of a textfile into a string-list.
        /// </summary>
        /// <returns>A string-list containing all lines of the given textfile.</returns>
        public List<string> ReadAllLines()
        {
            if (!File.Exists(this.FileName))
            {
                throw new ArgumentException(String.Format($"'{this.FileName}' wurde nicht gefunden!"));
            }
            return new List<string>(File.ReadAllLines(this.FileName));
        }

        /// <summary>
        /// Constructor - accepts the path to the Textfile to be read into a string-list..
        /// </summary>
        /// <param name="fileName">Path to the Textfile to be read into a string-list.</param>
        public TextFileReader(string fileName = "")
        {
            this.FileName = fileName;
        }
    }
}
