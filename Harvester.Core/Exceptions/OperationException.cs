using System;

namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    public class OperationException : Exception
    {
        public OperationException(String message)
            : base(message)
        { }

        public OperationException(String message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
