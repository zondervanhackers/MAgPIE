using System;
using System.Data.Linq;
using System.IO;
using System.Data.Common;

namespace ZondervanLibrary.SharedLibrary.Repository
{
    /// <summary>
    /// Provides a wrapper around the public methods and properties of <see cref="System.Data.Linq.DataContext"/>.
    /// </summary>
    public interface IDataContext : IDisposable
    {
        /// <summary>
        /// Gets a collection of objects that caused concurrency conflicts when SubmitChanges was called.
        /// </summary>
        ChangeConflictCollection ChangeConflicts { get; }

        /// <summary>
        /// Gets or sets a value that increases the time-out period for queries that would otherwise time out during the default time-out period.
        /// </summary>
        /// <remarks>This property gets or sets the command time-out used to execute generated commands (IDbCommands).  When this property is not set, the default value of CommandTimeout is used for query command execution. This default value is set by the storage provider. Note that some providers may throw exceptions if this value is set to a non-zero value.</remarks>
        Int32 CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the destination to write the SQL query or command.
        /// </summary>
        /// <remarks>Set this property to null to disable command logging.</remarks>
        TextWriter Log { get; set; }

        /// <summary>
        /// Returns a collection of objects of a particular type, where the type is defined by the type parameter.
        /// </summary>
        /// <param name="type">The type of the objects to be returned.</param>
        /// <returns>A collection of objects defined by the type parameter.</returns>
        /// <remarks>This is a weakly typed version of GetTable. It is important to have a weakly typed version because it is a relatively common practice to construct queries dynamically. It would be inconvenient to force the application to use reflection to call the correct generic method.</remarks>
        ITable GetTable(Type type);

        /// <summary>
        /// Returns a collection of objects of a particular type, where the type is defined by the TEntity parameter.
        /// </summary>
        /// <typeparam name="TEntity">The type of the objects to be returned.</typeparam>
        /// <returns>A collection of objects defined by the TEntity parameter. </returns>
        /// <remarks>This method is the main entry point for querying. When a strongly typed DataContext is created, new generated properties encapsulate calls to this method. For example, a Customers property is generated that returns GetTable{Customer}.</remarks>
        Table<TEntity> GetTable<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Computes the set of modified objects to be inserted, updated, or deleted, and executes the appropriate commands to implement the changes to the database.
        /// </summary>
        /// <remarks>If override methods are present for insert, update, or delete, SubmitChanges executes these methods instead of the default LINQ to SQL commands.</remarks>
        void SubmitChanges();

        /// <summary>
        /// Sends changes that were made to retrieved objects to the underlying database, and specifies the action to be taken if the submission fails.
        /// </summary>
        /// <param name="failureMode">The action to be taken if the submission fails.</param>
        /// <remarks>Default failure mode is FailOnFirstConflict.</remarks>
        void SubmitChanges(ConflictMode failureMode);

        /// <summary>
        /// Executes SQL commands directly on the database.
        /// </summary>
        /// <param name="command">The SQL command to be executed.</param>
        /// <param name="parameters">The array of parameters to be passed to the command.</param>
        /// <returns>The number of rows modified by the executed command.</returns>
        int ExecuteCommand(String command, params object[] parameters);

        /// <summary>
        /// Gets the connection used by the framework.
        /// </summary>
        DbConnection Connection { get; }
    }
}
