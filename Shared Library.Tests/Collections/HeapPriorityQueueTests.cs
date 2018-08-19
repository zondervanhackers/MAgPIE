using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    public class HeapPriorityQueueTests : IPriorityQueueTests
    {
        public override IPriorityQueue<T> CreateInstance<T>()
        {
            return new HeapPriorityQueue<T>();
        }
    }
}
