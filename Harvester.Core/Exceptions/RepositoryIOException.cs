using System;
using ZondervanLibrary.Harvester.Core.Repository;

namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    /// <summary>
    /// Provides a class for exceptions that occur in a repository that are caused by the underlying transport mechanism or by the host.
    /// </summary>
    /// <remarks>
    ///     <para>The action item to be taken when this type of exception occurs is to wait and the retry the operation after the number of seconds specified by <see cref="RetryWaitTime"/>.</para>
    /// </remarks>
    [Serializable]
    public class RepositoryIOException : RepositoryException
    {
        private readonly IOExceptionCategory _category;
        private readonly Int32 _retryWaitTime;

        public RepositoryIOException(IOExceptionCategory category, Int32 retryWaitTime, IRepository repository, String message)
            : base(repository, message)
        {
            _category = category;
            _retryWaitTime = retryWaitTime;
        }

        public RepositoryIOException(IOExceptionCategory category, Int32 retryWaitTime, IRepository repository, String message, Exception innerException)
            : base(repository, message, innerException)
        {
            _category = category;
            _retryWaitTime = retryWaitTime;
        }

        public IOExceptionCategory Category => _category;

        public Int32 RetryWaitTime => _retryWaitTime;
    }
}
