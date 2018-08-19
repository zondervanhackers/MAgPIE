using System;
using ZondervanLibrary.Harvester.Core.Repository;

namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    /// <summary>
    /// Provides a base class for exceptions that occur in a repository that are caused by an error in the implementation of the repository.
    /// </summary>
    /// <remarks>
    ///     <para>The action item to be taken when this type of exception occurs is to flag all operations using this repository as unstable (and thus unusable) and to notify the development team.</para>
    /// </remarks>
    [Serializable]
    public class RepositoryImplementationException : RepositoryException
    {
        private readonly ImplementationExceptionCategory _category;

        public RepositoryImplementationException(ImplementationExceptionCategory category, IRepository repository, String message)
            : base(repository, message)
        {
            _category = category;
        }

        public RepositoryImplementationException(ImplementationExceptionCategory category, IRepository repository, String message, Exception innerException)
            : base(repository, message, innerException)
        {
            _category = category;
        }

        public ImplementationExceptionCategory Category => _category;
    }
}
