using System;
using System.Collections.Generic;
using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    public abstract class IPriorityQueueTests
    {
        public abstract IPriorityQueue<T> CreateInstance<T>()
            where T : IComparable<T>;

        public static IEnumerable<Object[]> GenerateLists(int count, int minSize, int maxSize)
        {
            int i = 0;
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            while (i++ < count)
            {
                List<Int32> list = new List<Int32>();

                int j = 0;

                int size = random.Next(minSize, maxSize);

                while (j++ < size)
                {
                    list.Add(random.Next());
                }

                yield return new object[] { list };
            }
        }

        #region Count

        //[Theory]
        //[MemberData("GenerateLists", 3, 0, 25)]
        //public void Count_Should_Return_Correct_Count(IEnumerable<Int32> numbers)
        //{
        //    // Arrange
        //    IPriorityQueue<Int32> queue = CreateInstance<Int32>();
        //    queue.EnqueueAll(numbers.ToList());

        //    // Act
        //    int result = queue.Count;

        //    // Assert
        //    Assert.Equal(numbers.Count(), result);
        //}

        //#endregion

        //#region Enqueue

        //[Fact]
        //public void Enqueue_Should_Throw_ArgumentNullException_If_Item_Null()
        //{
        //    // Arrange
        //    IPriorityQueue<String> queue = CreateInstance<String>();

        //    // Act/Assert
        //    Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null));
        //}

        //[Theory]
        //[InlineData(0)]
        //[InlineData(1)]
        //[InlineData(2)]
        //[InlineData(3)]
        //[InlineData(4)]
        //[InlineData(5)]
        //[InlineData(6)]
        //public void Enqueue_GetEnumerator_Pairing_Should_Be_Correct(Int32 itemToAdd)
        //{
        //    // Arrange
        //    List<Int32> seedArray = new List<Int32>() { 1, 3, 5 };
        //    IPriorityQueue<Int32> queue = CreateInstance<Int32>();
        //    queue.EnqueueAll(seedArray);

        //    // Act
        //    queue.Enqueue(itemToAdd);

        //    // Assert
        //    var sorted = queue.ToList();
        //    sorted.Sort();
        //    Assert.Equal(queue, sorted);
        //}

        //#endregion

        //#region EnqueueAll

        //[Fact]
        //public void EnqueueAll_Should_Throw_ArgumentNullException_If_Items_Null()
        //{
        //    // Arrange
        //    IPriorityQueue<String> queue = CreateInstance<String>();

        //    // Act/Assert
        //    Assert.Throws<ArgumentNullException>(() => queue.EnqueueAll(null));
        //}

        //[Fact]
        //public void EnqueueAll_Should_Throw_ArgumentNullException_If_Items_Contains_A_Null_Element()
        //{
        //    // Arrange
        //    IPriorityQueue<String> queue = CreateInstance<String>();
        //    List<String> items = new List<String>() { "a", "b", null, "c" };

        //    // Act/Assert
        //    Assert.Throws<ArgumentNullException>(() => queue.EnqueueAll(items));
        //}

        //[Theory]
        //[MemberData("GenerateLists", 3, 0, 25)]
        //public void EnqueueAll_GetEnumerator_Pairing_Should_Be_Correct(IEnumerable<Int32> numbers)
        //{
        //    // Arrange
        //    IPriorityQueue<Int32> queue = CreateInstance<Int32>();

        //    // Act
        //    queue.EnqueueAll(numbers.ToList());

        //    // Assert
        //    var sorted = queue.ToList();
        //    sorted.Sort();
        //    Assert.Equal(queue, sorted);
        //}

        //#endregion

        //#region Dequeue

        //[Fact]
        //public void Dequeue_Should_Throw_InvalidOperationException_If_Queue_Is_Empty()
        //{
        //    // Arrange
        //    IPriorityQueue<Int32> queue = CreateInstance<Int32>();

        //    // Act/Assert
        //    Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        //}

        //[Theory]
        //[MemberData("GenerateLists", 3, 1, 25)]
        //public void Dequeue_Should_Return_Smallest_Element_In_Queue(IEnumerable<Int32> numbers)
        //{
        //    // Arrange
        //    List<Int32> seedArray = numbers.ToList();
        //    IPriorityQueue<Int32> queue = CreateInstance<Int32>();
        //    queue.EnqueueAll(seedArray);

        //    // Act/Assert
        //    Int32 previous = queue.Dequeue();

        //    while (queue.Count > 0)
        //    {
        //        Int32 current = queue.Dequeue();

        //        Assert.True(previous < current);

        //        previous = current;
        //    }
        //}

        #endregion
    }
}
