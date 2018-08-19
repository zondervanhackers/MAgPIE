using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Xunit;

using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    public abstract class ISlaveObservableCollectionTests
    {
        public abstract ISlaveObservableCollection<T> CreateInstance<T>(Func<IEnumerable<T>> getEnumerable);

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Add_With_Index(Int32 index)
        {
            // Arrange
            List<Int32> master = new List<Int32>() { 1, 2 };
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            int newItem = 7;
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, index);

            // Act
            master.Insert(index, newItem);
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }

        [Fact]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Add_Without_Index()
        {
            // Arrange
            List<Int32> master = new List<Int32> { 1, 2 };
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            int newItem = 7;
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem);

            // Act
            master.Add(newItem);
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 0)]
        [InlineData(2, 1)]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Move(int oldIndex, int newIndex)
        {
            // Arrange
            ObservableCollection<Int32> master = new ObservableCollection<Int32>() { 1, 2, 3 };
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, master[oldIndex], newIndex, oldIndex);

            // Act
            master.Move(oldIndex, newIndex);
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Remove_With_Index(int index)
        {
            // Arrange
            List<Int32> master = new List<Int32>() { 1, 2, 3 };
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, master[index], index);

            // Act
            master.RemoveAt(index);
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Remove_Without_Index(int value)
        {
            // Arrange
            List<Int32> master = new List<Int32>() { 1, 2, 3 };
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value);

            // Act
            master.Remove(value);
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Replace_With_Index(int index)
        {
            // Arrange
            List<Int32> master = new List<Int32>() { 1, 2, 3 };
            Int32 newValue = 7;
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, (object)master[index], index);

            // Act
            master[index] = newValue;
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Replace_Without_Index(int value)
        {
            // Arrange
            List<Int32> master = new List<Int32>() { 1, 2, 3 };
            Int32 newValue = 7;
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, (object)value);

            // Act
            master[master.IndexOf(value)] = newValue;
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }

        [Fact]
        public void OnMasterCollectionChanged_Should_Maintain_State_After_Master_Reset()
        {
            // Arrange
            List<Int32> master = new List<Int32>() { 1, 2, 3 };
            ISlaveObservableCollection<Int32> slave = CreateInstance(() => master);
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            // Act
            master.Add(7);
            master[0] = 5;
            slave.OnMasterCollectionChanged(args);

            // Assert
            Assert.Equal(master, slave);
        }
    }
}