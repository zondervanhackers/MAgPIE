using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, Boolean> _equals;

        public EqualityComparer(Func<T, T, Boolean> equals)
        {
            Contract.Requires(equals != null);

            _equals = equals;
        }

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class EqualityComparer
    {
        public static EqualityComparer<T> Wrap<T>(Func<T, T, Boolean> equals)
        {
            return new EqualityComparer<T>(equals);
        }
    }
}
