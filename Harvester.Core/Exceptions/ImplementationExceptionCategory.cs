namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    public enum ImplementationExceptionCategory
    {
        /// <summary>
        /// Used when the repository encounters an exception it does not recognize.
        /// </summary>
        UnrecognizedException,

        /// <summary>
        /// Used when the repository does not recognize the protocol.
        /// </summary>
        UnrecognizedProtocol
    }
}
