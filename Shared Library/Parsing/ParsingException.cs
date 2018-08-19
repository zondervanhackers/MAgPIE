using System;

namespace ZondervanLibrary.SharedLibrary.Parsing
{
    public class ParsingException : Exception
    {
        private readonly Int32 _lineNumber;

        public ParsingException(Int32 lineNumber)
            : base()
        {
            _lineNumber = lineNumber;
        }

        public ParsingException(Int32 lineNumber, String message)
            : base(message)
        {
            _lineNumber = lineNumber;
        }

        public ParsingException(Int32 lineNumber, String message, Exception innerException)
            : base(message, innerException)
        {
            _lineNumber = lineNumber;
        }
    }
}
