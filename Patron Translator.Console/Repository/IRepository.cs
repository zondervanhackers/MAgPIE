using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace ZondervanLibrary.PatronTranslator.Console.Repository
{
    /// <summary>
    /// An interface intended to encapsulate the DataContext and Table classes.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
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
        /// Returns a representation of the repository that is queryable.
        /// </summary>
        /// <returns>An IQueryable<TEntity> that represents the repository.</returns>
        IQueryable<TEntity> AsQueryable();

        /// <summary>
        /// Attaches a disconnected or "detached" entity to a new DataContext when original values are required for optimistic concurrency checks.
        /// </summary>
        /// <param name="entity">The original values of the entity to be attached.</param>
        /// <remarks>Use the Attach methods with entities that have been created in one DataContext, serialized to a client, and then deserialized back to perform an update or delete operation. Because the new DataContext has no way of tracking what the original values were for a disconnected entity, the client is responsible for supplying those values. In this version of Attach, the entity is assumed to be in its original value state. After calling this method, you can then update its fields, for example with additional data sent from the client.</remarks>
        void Attach(TEntity entity);

        /// <summary>
        /// Attaches an entity to the DataContext in either a modified or unmodified state.
        /// </summary>
        /// <param name="entity">The entity to be attached.</param>
        /// <param name="asModified">true to attach the entity as modified; false to attach the entity as unmodified.</param>
        /// <remarks>If attaching as modified, the entity must either declare a version member or must not participate in update conflict checking. When a new entity is attached, deferred loaders for any child collections (for example, EntitySet collections of entities from associated tables) are initialized. When SubmitChanges is called, members of the child collections are put into an Unmodified state. To update members of a child collection, you must explicitly call Attach and specify that entity.</remarks>
        void Attach(TEntity entity, Boolean asModified);

        /// <summary>
        /// Attaches an entity to the DataContext in either a modified or unmodified state by specifying both the entity and its original state.
        /// </summary>
        /// <param name="entity">The entity to be attached.</param>
        /// <param name="original">An instance of the same entity type with data members that contain the original values.</param>
        /// <remarks>In the following example, the Customer object is already correctly configured. You can call Attach without having to replay the updates.</remarks>
        void Attach(TEntity entity, TEntity original);

        /// <summary>
        /// Attaches all entities of a collection to the DataContext in either a modified or unmodified state.
        /// </summary>
        /// <typeparam name="TSubEntity">The type of entities to attach.</typeparam>
        /// <param name="entities">The collection of entities.</param>
        /// <remarks>This method attaches all entities of a collection to a new DataContext. When a new entity is attached, deferred loaders for any child collections (for example, EntitySet collections of entities from associated tables) are initialized. When SubmitChanges is called, members of the child collections are put into an Unmodified state. To update members of a child collection, you must explicitly call Attach and specify that entity.</remarks>
        void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity;

        /// <summary>
        /// Attaches all entities of a collection to the DataContext in either a modified or unmodified state.
        /// </summary>
        /// <typeparam name="TSubEntity">The type of entities to attach.</typeparam>
        /// <param name="entities">The collection of entities.</param>
        /// <param name="asModified">true if the object has a timestamp or RowVersion member; false if original values are being used for the optimistic concurrency check.</param>
        /// <remarks>This method attaches all entities of a collection to the DataContext in either a modified or unmodified state. If attaching as modified, the entity must either declare a version member or must not participate in update conflict checking. If attaching as unmodified, the entity is assumed to represent the original value. After calling this method, the entity's fields can be modified with other information from the client before SubmitChanges is called.</remarks>
        void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, Boolean asModified)
            where TSubEntity : TEntity;

        /// <summary>
        /// Puts all entities from the collection into a pending delete state.
        /// </summary>
        /// <typeparam name="TSubEntity">The type of the elements to delete.</typeparam>
        /// <param name="entities">The entities to delete.</param>
        /// <remarks>Entities that are put into the pending delete state with this method do not disappear from query results until after SubmitChanges is called. Disconnected entities must be attached before they can be deleted.</remarks>
        void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity;

        /// <summary>
        /// Puts an entity from this table into a pending delete state.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <remarks>The removed entity does not disappear from the query results until after SubmitChanges is called. Disconnected entities must first be attached before they can be deleted.</remarks>
        void DeleteOnSubmit(TEntity entity);

        /// <summary>
        /// Adds all entities of a collection to the DataContext in a pending insert state.
        /// </summary>
        /// <typeparam name="TSubEntity">The type of the elements to insert.</typeparam>
        /// <param name="entities">The entities to add.</param>
        /// <remarks>The added entities will not be in query results until after SubmitChanges has been called.</remarks>
        void InsertAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity;

        /// <summary>
        /// Adds an entity in a pending insert state to this Table(TEntity).
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <remarks>The added entity will not appear in query results from this table until after SubmitChanges has been called.</remarks>
        void InsertOnSubmit(TEntity entity);

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
    }
}
