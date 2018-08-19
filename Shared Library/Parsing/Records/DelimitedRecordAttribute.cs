using System;

namespace ZondervanLibrary.SharedLibrary.Parsing.Records
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class DelimitedRecordAttribute : Attribute, IRecordAttribute
    {
        public DelimitedRecordAttribute(String delimiter)
        {
            Delimiter = delimiter;
        }

        public String Delimiter { get; }

        /// <inheritdoc/>
        public Boolean IgnoreFirstLine { get; set; }
    }
}
