using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Windows.Threading;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    public class SlaveObservableCollection<T> : ISlaveObservableCollection<T>
    {
        private readonly Func<IEnumerable<T>> _getMasterEnumerable;
        private List<T> _collection;

        private NotifyCollectionChangedEventHandler _collectionChanged;
        private readonly object _onCollectionChangedLock = new object();
        private readonly object _collectionChangedLock = new object();

        /// <summary>
        /// Creates a new instance of <see cref="SlaveObservableCollection"/>.
        /// </summary>
        /// <param name="getMasterEnumerable"></param>
        public SlaveObservableCollection(Func<IEnumerable<T>> getMasterEnumerable)
        {
            Contract.Requires(getMasterEnumerable != null);

            _getMasterEnumerable = getMasterEnumerable;

            _collection = new List<T>(_getMasterEnumerable());
        }
        
        /// <inheritdoc/>
        public void OnMasterCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            // Underlying List<T> _collection is not thead-safe, therefore we lock.
            lock (_collection)
            {
                switch (eventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Contract.Assert(eventArgs.NewStartingIndex >= -1 && eventArgs.NewStartingIndex <= _collection.Count);
                        Contract.Assert(eventArgs.NewItems.Count == 1);

                        if (eventArgs.NewStartingIndex > -1)
                        {
                            _collection.Insert(eventArgs.NewStartingIndex, eventArgs.NewItems.Cast<T>().First());
                        }
                        else
                        {
                            _collection.Add(eventArgs.NewItems.Cast<T>().First());
                        }

                        break;

                    case NotifyCollectionChangedAction.Move:
                        Contract.Assert(eventArgs.OldStartingIndex >= 0 && eventArgs.OldStartingIndex < _collection.Count);
                        Contract.Assert(eventArgs.NewStartingIndex >= 0 && eventArgs.NewStartingIndex < _collection.Count);
                        Contract.Assert(eventArgs.NewItems.Count == 1);
                        Contract.Assert(eventArgs.OldItems.Count == 1);

                        T item = _collection[eventArgs.OldStartingIndex];
                        _collection.RemoveAt(eventArgs.OldStartingIndex);
                        _collection.Insert(eventArgs.NewStartingIndex, item);

                        break;

                    case NotifyCollectionChangedAction.Remove:
                        Contract.Assert(eventArgs.OldStartingIndex >= -1 && eventArgs.OldStartingIndex < _collection.Count);
                        Contract.Assert(eventArgs.OldItems.Count == 1);
                        Contract.Assert(_collection.Contains(eventArgs.OldItems.Cast<T>().First()));

                        if (eventArgs.OldStartingIndex > -1)
                        {
                            _collection.RemoveAt(eventArgs.OldStartingIndex);
                        }
                        else
                        {
                            _collection.Remove(eventArgs.OldItems.Cast<T>().First());
                        }

                        break;

                    case NotifyCollectionChangedAction.Replace:
                        Contract.Assert(eventArgs.OldStartingIndex >= -1 && eventArgs.OldStartingIndex < _collection.Count);
                        Contract.Assert(eventArgs.OldItems.Count == 1);
                        Contract.Assert(eventArgs.NewItems.Count == 1);
                        Contract.Assert(_collection.Contains(eventArgs.OldItems.Cast<T>().First()));

                        int index = (eventArgs.OldStartingIndex > -1) ? eventArgs.OldStartingIndex : _collection.IndexOf(eventArgs.OldItems.Cast<T>().First());

                        _collection.RemoveAt(index);
                        _collection.Insert(index, eventArgs.NewItems.Cast<T>().First());

                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _collection = new List<T>(_getMasterEnumerable());

                        break;
                }
            }

            OnCollectionChanged(eventArgs);
        }
        
        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock (_collectionChangedLock)
                {
                    _collectionChanged += value;
                }
            }
            remove
            {
                lock (_collectionChangedLock)
                {
                    _collectionChanged -= value;
                }
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            lock (_onCollectionChangedLock)
            {
                NotifyCollectionChangedEventHandler eventHandler = _collectionChanged;

                if (eventHandler != null)
                {
                    Delegate[] delegates = eventHandler.GetInvocationList();

                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        // Check if we need to invoke the handler on a different thread.
                        if (handler.Target is DispatcherObject dispatcherObject && dispatcherObject.CheckAccess() == false)
                        {
                            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, eventArgs);
                        }
                        else
                        {
                            handler(this, eventArgs);
                        }
                    }
                }
            }
        }

        public T this[int index] => _collection[index];

        public int Count => _collection.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}
