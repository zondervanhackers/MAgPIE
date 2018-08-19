using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    public abstract class IObservablePriorityQueueTests : IPriorityQueueTests
    {
        public abstract IObservablePriorityQueue<T> CreateObservableInstance<T>()
            where T : IComparable<T>;

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 1)]
        [InlineData(4, 2)]
        [InlineData(6, 3)]
        public void Enqueue_Should_Fire_NotifyCollectionChanged_Add_Event_With_Appropiate_Index(int number, int expectedIndex)
        {
            // Arrange
            IObservablePriorityQueue<int> queue = CreateObservableInstance<int>();
            queue.EnqueueAll(new List<int>() { 1, 3, 5 });

            NotifyCollectionChangedEventArgs args = null;

            queue.CollectionChanged += (sender, e) => {
                args = e;
            };

            // Act
            queue.Enqueue(number);

            // Assert
            Assert.NotNull(args);
            Assert.Equal(args.Action, NotifyCollectionChangedAction.Add);
            Assert.Equal(args.NewStartingIndex, expectedIndex);
            Assert.Equal(args.NewItems.Count, 1);
            Assert.Equal(args.NewItems[0], number);
        }

        [Fact]
        public void EnqueueAll_Should_Not_Fire_NotifyCollectionChanged_Event_With_Empty_Input()
        {
            // Arrange
            IObservablePriorityQueue<int> queue = CreateObservableInstance<int>();

            NotifyCollectionChangedEventArgs args = null;

            queue.CollectionChanged += (sender, e) =>
            {
                args = e;
            };

            // Act
            queue.EnqueueAll(new List<Int32>());

            // Assert
            Assert.Null(args);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 1)]
        [InlineData(4, 2)]
        [InlineData(6, 3)]
        public void EnqueueAll_Should_Fire_NotifyCollectionChanged_Add_Event_With_Input_Of_Count_One(int number, int expectedIndex)
        {
            // Arrange
            IObservablePriorityQueue<int> queue = CreateObservableInstance<int>();
            queue.EnqueueAll(new List<int>() { 1, 3, 5 });

            NotifyCollectionChangedEventArgs args = null;

            queue.CollectionChanged += (sender, e) =>
            {
                args = e;
            };

            // Act
            queue.EnqueueAll(new List<Int32>() { number });

            // Assert
            Assert.NotNull(args);
            Assert.Equal(args.Action, NotifyCollectionChangedAction.Add);
            Assert.Equal(args.NewStartingIndex, expectedIndex);
            Assert.Equal(args.NewItems.Count, 1);
            Assert.Equal(args.NewItems[0], number);
        }

        [Fact]
        public void EnqueueAll_Should_Fire_NotifyCollectionChanged_Reset_Event_With_Input_Of_Count_Greater_Than_One()
        {
            // Arrange
            IObservablePriorityQueue<int> queue = CreateObservableInstance<int>();

            NotifyCollectionChangedEventArgs args = null;

            queue.CollectionChanged += (sender, e) =>
            {
                args = e;
            };

            // Act
            queue.EnqueueAll(new List<Int32>() { 1, 2 });

            // Assert
            Assert.NotNull(args);
            Assert.Equal(args.Action, NotifyCollectionChangedAction.Reset);
        }

        [Fact]
        public void Dequeue_Should_Fire_NotifyCollectionChanged_Remove_Event()
        {
            // Arrange
            IObservablePriorityQueue<int> queue = CreateObservableInstance<int>();
            queue.EnqueueAll(new List<int>() { 1, 2, 3 });

            NotifyCollectionChangedEventArgs args = null;

            queue.CollectionChanged += (sender, e) =>
            {
                args = e;
            };

            // Act
            queue.Dequeue();

            // Assert
            Assert.NotNull(args);
            Assert.Equal(args.Action, NotifyCollectionChangedAction.Remove);
            Assert.Equal(args.OldStartingIndex, 0);
            Assert.Equal(args.OldItems[0], 1);
        }
    }
}
