using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    public class ComparableEnumerator<T> : IComparable<ComparableEnumerator<T>>
        where T : IComparable<T>
    {
        public ComparableEnumerator(IEnumerator<T> enumerator)
        {
            Contract.Requires(enumerator != null);

            Enumerator = enumerator;
        }

        public IEnumerator<T> Enumerator { get; }

        /// <inheritdoc/>
        public int CompareTo(ComparableEnumerator<T> other)
        {
            return Enumerator.Current.CompareTo(other.Enumerator.Current);
        }
    }
}
