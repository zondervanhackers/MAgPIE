namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    public enum ConfigurationExceptionCategory
    {
        /// <summary>
        /// Used when a directory does not exist that the configuration specifies should exist.
        /// </summary>
        DirecoryNotFound,

        /// <summary>
        /// Used when a file exists that should not exist according to the configuration.
        /// </summary>
        FileExists,

        /// <summary>
        /// Used when a file cannot be accessed because it is locked or in use by another process.
        /// </summary>
        FileLocked,

        /// <summary>
        /// Used when a file name is too long
        /// </summary>
        FileNameTooLong,

        /// <summary>
        /// Used when a file does not exist that the configuration specifies should exist.
        /// </summary>
        FileNotFound,

        /// <summary>
        /// Used when the credentials provided by the configuration fail to authenticate the Harvester.
        /// </summary>
        InvalidCredentials,

        /// <summary>
        /// Used when the specified host is not valid.
        /// </summary>
        InvalidHost,

        /// <summary>
        /// Thrown when file does not exists that the configuration specifies should exist.
        /// </summary>
        UnauthorizedAccess,

        /// <summary>
        /// Thrown when the repository does not support the Report
        /// </summary>
        NotSupported
    }
}
