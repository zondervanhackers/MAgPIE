using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    /// <summary>
    /// Provides an abstract implementation of <see cref="IEnumerableDiff<TSource>"/> with abstract hooks for actions to be taken.
    /// </summary>
    /// <typeparam name="TSource">The type of the records of the lists to be differentiated.</typeparam>
    /// <typeparam name="TResult">The type of the records of the returned differentiated list.</typeparam>
    /// <typeparam name="TKey">The type of the key that uniquely identifies records of type <typeparamref name="TSource"/>.</typeparam>
    public abstract class EnumerableDiffBase<TSource, TResult, TKey> : IEnumerableDiff<TSource, TResult>
        where TSource : class
        where TKey : IComparable<TKey>
    {
        /// <summary>
        /// When implemented in a derived class, this function should lift the key from a record.
        /// </summary>
        protected abstract Func<TSource, TKey> GetKey { get; }

        /// <summary>
        /// When implemented in a derived class, this function should convert the record into a <typeparamref name="TResult"/> that is marked for insertion.
        /// </summary>
        /// <remarks>
        ///     <para>This property can be set to null to indicate that nothing should be output when matching records are unchanged.</para>
        /// </remarks>
        protected abstract Func<TSource, TResult> OnRecordAdded { get; }

        /// <summary>
        /// When implemented in a derived class, this function should convert the record into a <typeparamref name="TResult"/> that is marked for deletion.
        /// </summary>
        /// <remarks>
        ///     <para>This property can be set to null to indicate that nothing should be output when a record from the old enumerable is removed.</para>
        /// </remarks>
        protected abstract Func<TSource, TResult> OnRecordRemoved { get; }

        /// <summary>
        /// When implemented in a derived class, this function should produce a <typeparamref name="TResult"/> from the differing records in each enumerable.
        /// </summary>
        /// <remarks>
        ///     <para>The first argument will be the record from the old enumerable, and the second from the new enumerable.</para>
        ///     <para>This property can be set to null to indicate that nothing should be output when matching records are updated.</para>
        /// </remarks>
        protected abstract Func<TSource, TSource, TResult> OnRecordChanged { get; }

        /// <summary>
        /// When implemented in a derived class, this function should produce a <typeparam name="TResult"/> from the unchanged records in each enumerable.
        /// </summary>
        /// <remarks>
        ///     <para>The first argument will be the record from the old enumerable, and the second from the new enumerable.</para>
        ///     <para>This property can be set to null to indicate that nothing should be output when matching records are unchanged.</para>
        /// </remarks>
        protected abstract Func<TSource, TSource, TResult> OnRecordUnchanged { get; }

        /// <inheritdoc/>
        public IEnumerable<TResult> ComputeDiff(IEnumerable<TSource> oldEnumerable, IEnumerable<TSource> newEnumerable)
        {
            Contract.Requires(oldEnumerable != null);
            Contract.Requires(newEnumerable != null);

            Dictionary<TKey, TSource> oldRecords;
            HashSet<TKey> uniqueNewRecords = new HashSet<TKey>();
            List<TResult> ret = new List<TResult>(Math.Max(oldEnumerable.Count(), newEnumerable.Count()));

            try
            {
                oldRecords = oldEnumerable.ToDictionary(GetKey);
            }
            catch (ArgumentException)
            {
                throw Argument.Exception(() => oldEnumerable, "{0} cannot contain duplicates of the same key.");
            }

            foreach (TKey key in newEnumerable.Select(GetKey))
            {
                if (!uniqueNewRecords.Add(key))
                {
                    throw Argument.Exception(() => newEnumerable, "{0} cannot contain duplicates of the same key.");
                }
            }

            return ComputDiffHelper(oldRecords, newEnumerable);
        }

        // Helper function to ComputeDiff... the top level ComputeDiff should thrown arguments exceptions (so that they occur immediately)
        private IEnumerable<TResult> ComputDiffHelper(Dictionary<TKey, TSource> oldDictionary, IEnumerable<TSource> newEnumerable)
        {
            foreach (TSource newRecord in newEnumerable)
            {
                TKey key = GetKey(newRecord);

                TSource oldRecord;
                if (oldDictionary.TryGetValue(key, out oldRecord))
                {
                    // Record present in both collections
                    oldDictionary.Remove(key);

                    if (oldRecord.Equals(newRecord))
                    {
                        // Record is unchanged
                        if (OnRecordUnchanged != null)
                        {
                            yield return OnRecordUnchanged(oldRecord, newRecord);
                        }
                    }
                    else
                    {
                        // Record is changed
                        if (OnRecordChanged != null)
                        {
                            yield return OnRecordChanged(oldRecord, newRecord);
                        }
                    }
                }
                else
                {
                    // Record is new
                    if (OnRecordAdded != null)
                    {
                        yield return OnRecordAdded(newRecord);
                    }
                }
            }

            // Iterate over records to be removed.
            if (OnRecordRemoved != null)
            {
                foreach (TSource oldRecord in oldDictionary.Values)
                {
                    yield return OnRecordRemoved(oldRecord);
                }
            }
        }
    }
}
