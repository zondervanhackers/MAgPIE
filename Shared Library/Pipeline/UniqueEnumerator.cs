using System;
using System.Collections;
using System.Collections.Generic;

namespace ZondervanLibrary.SharedLibrary.Pipeline
{
    public class UniqueEnumerator<T, TUnique> : IEnumerator<T>
    {
        private readonly HashSet<TUnique> _elementsSeen;
        private readonly IEnumerator<T> _input;
        private readonly Func<T, TUnique> _uniqueField;

        public UniqueEnumerator(IEnumerator<T> input, Func<T, TUnique> uniqueField)
        {
            _input = input;
            _uniqueField = uniqueField;
            _elementsSeen = new HashSet<TUnique>();
        }

        public T Current => _input.Current;

        object IEnumerator.Current => _input.Current;

        public void Dispose()
        {
            _input.Dispose();
        }

        public Boolean MoveNext()
        {
            do
            {
                if (!_input.MoveNext())
                    return false;

            } while (_elementsSeen.Contains(_uniqueField(_input.Current)));

            _elementsSeen.Add(_uniqueField(_input.Current));

            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
