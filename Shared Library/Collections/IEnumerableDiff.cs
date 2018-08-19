using System.Collections.Generic;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    /// <summary>
    /// Defines an interface for a class that computs the diff between two homogenous enumerables.
    /// </summary>
    /// <typeparam name="TSource">The type of object that is being differentiated.</typeparam>
    public interface IEnumerableDiff<TSource, TResult>
    {
        /// <summary>
        /// Computes the diff between two homogenous enumerables.
        /// </summary>
        /// <param name="oldEnumerable">The old collection of elements.</param>
        /// <param name="newEnumerable">The new collection of elements.</param>
        /// <returns>An enumerable representing the changes that occured between the two collections.</returns>
        IEnumerable<TResult> ComputeDiff(IEnumerable<TSource> oldEnumerable, IEnumerable<TSource> newEnumerable);
    }
}
