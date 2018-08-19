using System;
using System.Diagnostics.Contracts;

using ZondervanLibrary.Harvester.Core.Repository;

namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    /// <summary>
    /// Provides a base class for exceptions that can occur from a repository.
    /// </summary>
    [Serializable]
    public abstract class RepositoryException : Exception
    {
        private readonly IRepository _repository;

        protected RepositoryException(IRepository repository, String message)
            : base(message)
        {
            Contract.Requires(repository != null);
            Contract.Requires(message != null);

            _repository = repository;
        }

        protected RepositoryException(IRepository repository, String message, Exception innerException)
            : base(message, innerException)
        {
            Contract.Requires(repository != null);
            Contract.Requires(message != null);
            Contract.Requires(innerException != null);

            _repository = repository;
        }

        // TODO: Flush out serialization properties.
    }
}
