using System;
using System.Collections.Generic;
using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    public class SlaveObservableCollectionTests : ISlaveObservableCollectionTests
    {
        public override ISlaveObservableCollection<T> CreateInstance<T>(Func<IEnumerable<T>> getEnumerable)
        {
            return new SlaveObservableCollection<T>(getEnumerable);
        }
    }
}
