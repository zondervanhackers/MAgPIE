using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    /// <inheritdoc/>
    public class ObservablePriorityQueue<T> : IObservablePriorityQueue<T>
        where T : IComparable<T>
    {
        private readonly List<T> _queue;

        public ObservablePriorityQueue()
        {
            _queue = new List<T>();
            _onCollectionChangedLock = new object();
        }

        /// <inheritdoc/>
        public Int32 Count => _queue.Count;

        /// <inheritdoc/>
        public T Peek()
        {
            lock (_queue)
            {
                if (_queue.Count == 0)
                    throw new InvalidOperationException();

                return _queue[0];
            }
        }

        /// <inheritdoc/>
        public void Enqueue(T item)
        {
            if (item == null)
                throw Argument.NullException(() => item);

            int index;

            lock (_queue)
            {
                index = Array.BinarySearch(_queue.ToArray(), item);

                index = (index < 0) ? ~index : index;

                _queue.Insert(index, item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <inheritdoc/>
        public void EnqueueAll(IEnumerable<T> items)
        {
            if (items == null)
                throw Argument.NullException(() => items);

            if (!typeof(T).IsValueType && items.Contains(default(T)))
                throw Argument.NullException(() => items);

            if (items.Count() == 1)
            {
                Enqueue(items.First());
            }
            else if (items.Count() > 1)
            {
                lock (_queue)
                {
                    _queue.AddRange(items);

                    _queue.Sort();
                }

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <inheritdoc/>
        public T Dequeue()
        {
            T item;

            lock (_queue)
            {
                if (_queue.Count == 0)
                    throw new InvalidOperationException();

                item = _queue[0];

                _queue.RemoveAt(0);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));

            return item;
        }

        #region IEnumerable<T>

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            List<T> copy;

            lock (_queue)
            {
                copy = new List<T>(_queue);
            }

            return copy.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected object _onCollectionChangedLock;

        /// <summary>
        /// Provides a thread-safe means of notfying all subscribers.
        /// </summary>
        /// <param name="e">The event args to send.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            lock (_onCollectionChangedLock)
            {
                NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;

                if (eventHandler != null)
                {
                    Delegate[] delegates = eventHandler.GetInvocationList();

                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        // Check if we need to invoke the handler on a different thread.
                        if (handler.Target is DispatcherObject dispatcherObject && dispatcherObject.CheckAccess() == false)
                        {
                            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                        }
                        else
                        {
                            handler(this, e);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
