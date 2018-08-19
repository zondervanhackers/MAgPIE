namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    public enum IOExceptionCategory
    {
        /// <summary>
        /// Used when the underlying connection timed out.
        /// </summary>
        TimedOut,

        /// <summary>
        /// Used when the network is required and unavailable.
        /// </summary>
        NetworkUnavailable,

        /// <summary>
        /// Used a general exception occurs in the underlying connection.
        /// </summary>
        General
    }
}
