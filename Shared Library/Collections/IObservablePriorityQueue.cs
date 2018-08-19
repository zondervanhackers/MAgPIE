using System;
using System.Collections.Specialized;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    /// <summary>
    /// The interface for a priority queue that will notify when its elements are changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservablePriorityQueue<T> : IPriorityQueue<T>, INotifyCollectionChanged
        where T : IComparable<T>
    { }
}
