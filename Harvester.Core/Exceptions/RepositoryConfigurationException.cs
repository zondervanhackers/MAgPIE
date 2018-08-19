using System;
using ZondervanLibrary.Harvester.Core.Repository;

namespace ZondervanLibrary.Harvester.Core.Exceptions
{
    /// <summary>
    /// Provides a class for exceptions occuring in a repository that are caused by a configuration setting.
    /// </summary>
    /// <remarks>
    ///     <para>The default action for a <see cref="RepositoryConfigurationException"/> should be to notify the user of the problem.</para>
    /// </remarks>
    [Serializable]
    public class RepositoryConfigurationException : RepositoryException
    {
        public RepositoryConfigurationException(ConfigurationExceptionCategory category, IRepository repository, String message)
            : base(repository, message)
        {
            Category = category;
        }

        public RepositoryConfigurationException(ConfigurationExceptionCategory category, IRepository repository, String message, Exception innerException)
            : base(repository, message, innerException)
        {
            Category = category;
        }

        public ConfigurationExceptionCategory Category { get; }
    }
}
