using System;

namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    public class ConfigurationFileException : Exception
    {
        public ConfigurationFileException(string message)
            : base(message)
        { }

        public ConfigurationFileException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
