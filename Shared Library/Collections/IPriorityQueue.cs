using System;
using System.Collections.Generic;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    /// <summary>
    /// The interface for a Priority Queue.
    /// </summary>
    /// <typeparam name="T">The type that this collection encapuslates; it must implement <see cref="IComparable"/> to allow for comparisons.</typeparam>
    /// <remarks>
    /// The implementation will be a Minimum Priority Queue, thus the smallest element in the queue will be returned when Dequeue or Peek is called.
    /// </remarks>
    public interface IPriorityQueue<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// The number of elements currently in the queue.
        /// </summary>
        Int32 Count { get; }

        /// <summary>
        /// Returns the smallest item in the priority queue without removing it.
        /// </summary>
        /// <returns>The smallest item in the priority queue.</returns>
        /// <exception cref="System.InvalidOperationException">Peek throws a <see cref="System.InvalidOperationException"/> when the PriorityQueue is empty.</exception>
        T Peek();

        /// <summary>
        /// Inserts the passed item into the priority queue.
        /// </summary>
        /// <param name="item">The item to insert into the priority queue.</param>
        /// <exception cref="System.ArgumentNullException">Enqueue throws a <see cref="System.ArgumentNullException"/> when <paramref name="item"/> is null.</exception>
        void Enqueue(T item);

        /// <summary>
        /// Removes and returns the smallest item in the priority queue.
        /// </summary>
        /// <param name="items">A collection of the items to insert into the priority queue.</param>
        /// <exception cref="System.ArgumentNullException">EnqueueAll throws a <see cref="System.ArgumentNullException"/> when items is null or when any of its individual items are null.</exception>
        void EnqueueAll(IEnumerable<T> items);

        /// <summary>
        /// Removes and returns the smallest item in the priority queue.
        /// </summary>
        /// <returns>The smallest item in the priority queue.</returns>
        /// <exception cref="System.InvalidOperationException">Dequeue throws a <see cref="System.InvalidOperationException"/> when the PriorityQueue is empty.</exception>
        T Dequeue();
    }
}
