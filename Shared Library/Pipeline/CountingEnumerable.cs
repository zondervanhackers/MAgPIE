using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Pipeline
{
    public class CountingEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public CountingEnumerable(IEnumerable<T> enumerable)
        {
            Contract.Requires(enumerable != null);

            _enumerable = enumerable;
        }

        public Int32 Count { get; private set; }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            Count = 0;

            foreach (T item in _enumerable)
            {
                Count++;
                yield return item;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class CountingEnumerable
    {
        public static CountingEnumerable<T> Wrap<T>(IEnumerable<T> enumerable)
        {
            return new CountingEnumerable<T>(enumerable);
        }
    }
}
