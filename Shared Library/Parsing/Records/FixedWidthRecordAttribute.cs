using System;

namespace ZondervanLibrary.SharedLibrary.Parsing.Records
{
    public class FixedWidthRecordAttribute : Attribute, IRecordAttribute
    {
        /// <inheritdoc/>
        public Boolean IgnoreFirstLine { get; set; }
    }
}
