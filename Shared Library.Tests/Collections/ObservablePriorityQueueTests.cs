using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    public class ObservablePriorityQueueTests : IObservablePriorityQueueTests
    {
        public override IPriorityQueue<T> CreateInstance<T>()
        {
            return new ObservablePriorityQueue<T>();
        }

        public override IObservablePriorityQueue<T> CreateObservableInstance<T>()
        {
            return new ObservablePriorityQueue<T>();
        }
    }
}
