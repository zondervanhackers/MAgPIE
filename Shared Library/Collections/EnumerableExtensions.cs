using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using ZondervanLibrary.SharedLibrary.Repository;
using System.Collections;

namespace ZondervanLibrary.SharedLibrary.Collections
{
    public static class EnumerableExtensions
    {
        public static IDataReader ToDataReader<T>(this IEnumerable<T> collection, Func<T, object[]> converter)
        {
            return new EnumerableDataReader<T>(collection, converter);
        }

        //public static IEnumerable<T> Counter<T>(this IEnumerable<T> collection, out int count)
        //{
        //    count = 0;

        //    foreach (var element in collection)
        //    {
        //        count++;
        //        yield return element;
        //    }
        //}

        public static void Fork<T>(this IEnumerable<T> collection, params Action<IEnumerable<T>>[] actions)
        {
            CountdownEvent countdown = new CountdownEvent(actions.Length);
            ForkedEnumerable<T> enumerable = new ForkedEnumerable<T>(collection, countdown);

            Task[] tasks = actions.Select(a => Task.Factory.StartNew(c =>
            {
                a(c as IEnumerable<T>);
                
                countdown.Signal();

            }, enumerable)).ToArray();

            Task.WaitAll(tasks); 
        }

        /// <summary>
        /// Projects each element of a sequence into a new form allowing for callback that be called on completion (by well behaved consumers).
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> collection, Func<TSource, Tuple<TResult, Action>> selector)
        {
            foreach (TSource element in collection)
            {
                Tuple<TResult, Action> result = selector(element);

                try
                {
                    yield return result.Item1;
                }
                finally
                {
                    result.Item2();
                }
            }
        }
    }

    public class ForkedEnumerable<T> : IEnumerable<T>
    {
        private readonly ForkedEnumerator<T> _enumerator;

        public ForkedEnumerable(IEnumerable<T> collection, CountdownEvent countdown)
        {
            _enumerator = new ForkedEnumerator<T>(collection, countdown);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }

    public class ForkedEnumerator<T> : IEnumerator<T>
    {
        private Task<bool> _moveNextTask;
        private readonly CountdownEvent _countdown;
        private readonly IEnumerator<T> _enumerator;

        public ForkedEnumerator(IEnumerable<T> collection, CountdownEvent countdown)
        {
            _countdown = countdown;
            _enumerator = collection.GetEnumerator();

            _moveNextTask = GetTask();
        }

        private Task<bool> GetTask()
        {
            return Task.Factory.StartNew(() =>
            {
                _countdown.Wait();
                _countdown.Reset();

                Boolean result = _enumerator.MoveNext();

                if (result)
                {
                    _moveNextTask = GetTask();
                }

                return result;
            });
        }

        public T Current => _enumerator.Current;

        object IEnumerator.Current => _enumerator.Current;

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            _countdown.Signal(); 

            return _moveNextTask.Result;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
