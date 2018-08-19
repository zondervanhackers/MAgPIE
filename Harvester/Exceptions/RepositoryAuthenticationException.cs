using System;

namespace ZondervanLibrary.Harvester.Exceptions
{
    /// <summary>
    /// An exception thrown when the repository is unable to authenticate.
    /// </summary>
    public class RepositoryAuthenticationException : Exception
    {
        public RepositoryAuthenticationException()
            : base()
        {

        }

        public RepositoryAuthenticationException(String message)
            : base(message)
        {

        }

        public RepositoryAuthenticationException(String message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
