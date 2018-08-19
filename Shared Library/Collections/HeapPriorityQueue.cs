using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    public class HeapPriorityQueue<T> : IPriorityQueue<T>
        where T : IComparable<T>
    {
        private readonly List<T> _heap;

        public HeapPriorityQueue()
        {
            _heap = new List<T>();
        }

        public HeapPriorityQueue(IEnumerable<T> collection)
        {
            _heap = collection.ToList();
        }

        #region IPriorityQueue<T>

        /// <inheritdoc/>
        public int Count => _heap.Count;

        /// <inheritdoc/>
        public T Peek()
        {
            Contract.Requires(_heap.Count > 0);

            return _heap[0];
        }

        /// <inheritdoc/>
        public void Enqueue(T item)
        {
            Contract.Requires(item != null);

            _heap.Add(item);

            int index = _heap.Count - 1;

            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;

                T currentItem = _heap[index];
                T parentItem = _heap[parentIndex];

                if (currentItem.CompareTo(parentItem) < 0)
                {
                    _heap[index] = parentItem;
                    _heap[parentIndex] = currentItem;

                    index = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        /// <inheritdoc/>
        public void EnqueueAll(IEnumerable<T> items)
        {
            if (!items.Any())
                return;

            if (items.Count() > _heap.Count)
            {
                _heap.AddRange(items);

                for (int i = (_heap.Count / 2) - 1; i >= 0; i--)
                {
                    Heapify(i);
                }
            }
            else
            {
                foreach (T item in items)
                {
                    Enqueue(item);
                }
            }
        }

        /// <inheritdoc/>
        public T Dequeue()
        {
            Contract.Requires(_heap.Count > 0);

            T temp = _heap[0];

            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            Heapify(0);

            return temp;
        }

        #endregion IPriorityQueue<T>

        #region IEnumerable<T>

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            List<T> copy;

            // Make a copy so that the priority queue does not change while we are iterating over it.
            lock (_heap)
            {
                copy = new List<T>(_heap);
            }

            return copy.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<T>

        private void Heapify(int index)
        {
            while (true)
            {
                int leftIndex = (index + 1) * 2 - 1;
                int rightIndex = (index + 1) * 2;

                if (leftIndex < _heap.Count)
                {
                    int minIndex = leftIndex;
                    T minValue = _heap[leftIndex];

                    if (rightIndex < _heap.Count)
                    {
                        T rightValue = _heap[rightIndex];

                        if (minValue.CompareTo(rightValue) > 0)
                        {
                            minIndex = rightIndex;
                            minValue = rightValue;
                        }
                    }

                    T indexValue = _heap[index];
                    if (indexValue.CompareTo(minValue) > 0)
                    {
                        _heap[minIndex] = indexValue;
                        _heap[index] = minValue;

                        index = minIndex;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
