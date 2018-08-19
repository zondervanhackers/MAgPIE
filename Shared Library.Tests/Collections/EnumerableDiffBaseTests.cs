using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    internal class EnumerableDiffDerivedTest<TSource, TResult, TKey> : EnumerableDiffBase<TSource, TResult, TKey>
        where TSource : class
        where TKey : IComparable<TKey>
    {
        private readonly Func<TSource, TKey> _getKey;
        private readonly Func<TSource, TResult> _onRecordAdded;
        private readonly Func<TSource, TResult> _onRecordRemoved;
        private readonly Func<TSource, TSource, TResult> _onRecordUpdated;
        private readonly Func<TSource, TSource, TResult> _onRecordUnchanged;

        public EnumerableDiffDerivedTest(Func<TSource, TKey> getKey, Func<TSource, TResult> onRecordAdded, Func<TSource, TResult> onRecordRemoved, Func<TSource, TSource, TResult> onRecordChanged, Func<TSource, TSource, TResult> onRecordUnchanged)
        {
            _getKey = getKey;
            _onRecordAdded = onRecordAdded;
            _onRecordRemoved = onRecordRemoved;
            _onRecordUpdated = onRecordChanged;
            _onRecordUnchanged = onRecordUnchanged;
        }

        protected override Func<TSource, TKey> GetKey { get { return _getKey; } }

        protected override Func<TSource, TResult> OnRecordAdded { get { return _onRecordAdded; } }

        protected override Func<TSource, TResult> OnRecordRemoved { get { return _onRecordRemoved; } }

        protected override Func<TSource, TSource, TResult> OnRecordChanged { get { return _onRecordUpdated; } }

        protected override Func<TSource, TSource, TResult> OnRecordUnchanged { get { return _onRecordUnchanged; } }
    }

    public class EnumerableDiffBaseTests
    {
        [Fact]
        public void ComputeDiff_Should_Add_New_Records_To_Output_When_OnRecordAdded_Not_Null()
        {
            // Arrange
            String newRecord = "a";
            List<String> oldEnumerable = new List<String>();
            List<String> newEnumerable = new List<String>() { newRecord };

            var testClass = new EnumerableDiffDerivedTest<String, Int32, String>(
                getKey: s => s,
                onRecordAdded: s => s.GetHashCode(),
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal(newRecord.GetHashCode(), result.First());
        }

        [Fact]
        public void ComputeDiff_Should_Not_Add_New_Records_To_Output_When_OnRecordAdded_Is_Null()
        {
            // Arrange
            String newRecord = "a";
            List<String> oldEnumerable = new List<String>();
            List<String> newEnumerable = new List<String>() { newRecord };

            var testClass = new EnumerableDiffDerivedTest<String, Int32, String>(
                getKey: s => s,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

            // Assert
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void ComputeDiff_Should_Add_Removed_Records_To_Output_When_OnRecordRemoved_Not_Null()
        {
            // Arrange
            String oldRecord = "a";
            List<String> oldEnumerable = new List<String>() { oldRecord };
            List<String> newEnumerable = new List<String>();

            var testClass = new EnumerableDiffDerivedTest<String, Int32, String>(
                getKey: s => s,
                onRecordAdded: null,
                onRecordRemoved: s => s.GetHashCode(),
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal(oldRecord.GetHashCode(), result.First());
        }

        [Fact]
        public void ComputeDiff_Should_Not_Add_Removed_Records_To_Output_When_OnRecordRemoved_Is_Null()
        {
            // Arrange
            String oldRecord = "a";
            List<String> oldEnumerable = new List<String>() { oldRecord };
            List<String> newEnumerable = new List<String>();

            var testClass = new EnumerableDiffDerivedTest<String, Int32, String>(
                getKey: s => s,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

            // Assert
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void ComputeDiff_Should_Add_Changed_Records_To_Output_When_OnRecordChanged_Not_Null()
        {
            // Arrange
            var oldRecord = new Tuple<String, Int32>("a", 0);
            var newRecord = new Tuple<String, Int32>("a", 1);
            var oldEnumerable = new List<Tuple<String, Int32>>() { oldRecord };
            var newEnumerable = new List<Tuple<String, Int32>>() { newRecord };

            var testClass = new EnumerableDiffDerivedTest<Tuple<String, Int32>, Tuple<String, Int32>, String>(
                getKey: s => s.Item1,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: (a, b) => b,
                onRecordUnchanged: null 
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal(newRecord, result.First());
        }

        [Fact]
        public void ComputeDiff_Should_Not_Add_Changed_Records_To_Output_When_OnRecordChanged_Is_Null()
        {
            // Arrange
            var oldRecord = new Tuple<String, Int32>("a", 0);
            var newRecord = new Tuple<String, Int32>("a", 1);
            var oldEnumerable = new List<Tuple<String, Int32>>() { oldRecord };
            var newEnumerable = new List<Tuple<String, Int32>>() { newRecord };

            var testClass = new EnumerableDiffDerivedTest<Tuple<String, Int32>, Tuple<String, Int32>, String>(
                getKey: s => s.Item1,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

            // Assert
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void ComputeDiff_Should_Add_Unchanged_Record_To_Output_When_OnRecordUnchanged_Is_Not_Null()
        {
            // Arrange
            var oldRecord = new Tuple<String, Int32>("a", 0);
            var newRecord = new Tuple<String, Int32>("a", 0);
            var oldEnumerable = new List<Tuple<String, Int32>>() { oldRecord };
            var newEnumerable = new List<Tuple<String, Int32>>() { newRecord };

            var testClass = new EnumerableDiffDerivedTest<Tuple<String, Int32>, Tuple<String, Int32>, String>(
                getKey: s => s.Item1,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: (a, b) => b
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();
            
            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal(newRecord, result.First());
        }

        [Fact]
        public void ComputeDiff_Should_Not_Add_Unchanged_Records_To_Output_When_OnRecordUnchanged_Is_Null()
        {
            // Arrange
            var oldRecord = new Tuple<String, Int32>("a", 0);
            var newRecord = new Tuple<String, Int32>("a", 0);
            var oldEnumerable = new List<Tuple<String, Int32>>() { oldRecord };
            var newEnumerable = new List<Tuple<String, Int32>>() { newRecord };

            var testClass = new EnumerableDiffDerivedTest<Tuple<String, Int32>, Tuple<String, Int32>, String>(
                getKey: s => s.Item1,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

            // Assert
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void ComputeDiff_Should_Throw_ArgumentException_If_OldEnumerable_Contains_Duplicate_Keys()
        {
            // Arrange
            String value = "a";
            List<String> oldEnumerable = new List<String>() { value, value };
            List<String> newEnumerable = new List<String>();

            var testClass = new EnumerableDiffDerivedTest<String, Int32, String>(
                getKey: s => s,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = Record.Exception(() => testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray());
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.GetType(), typeof(ArgumentException));
        }

        [Fact]
        public void ComputeDiff_Should_Throw_ArgumentException_If_NewEnumerable_Contains_Duplicate_Keys()
        {
            // Arrange
            String value = "a";
            List<String> oldEnumerable = new List<String>();
            List<String> newEnumerable = new List<String>() { value, value }; 

            var testClass = new EnumerableDiffDerivedTest<String, Int32, String>(
                getKey: s => s,
                onRecordAdded: null,
                onRecordRemoved: null,
                onRecordChanged: null,
                onRecordUnchanged: null
                );

            // Act
            var result = Record.Exception(() => testClass.ComputeDiff(oldEnumerable, newEnumerable).ToArray());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.GetType(), typeof(ArgumentException));
        }
    }
}
