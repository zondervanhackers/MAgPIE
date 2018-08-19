using System;

namespace ZondervanLibrary.SharedLibrary.Parsing.Records
{
    public interface IRecordAttribute
    {
        /// <summary>
        /// Gets or sets whether the first line of the input should be ignored (e.g. it contains header information).
        /// </summary>
        Boolean IgnoreFirstLine { get; set; }
    }
}
