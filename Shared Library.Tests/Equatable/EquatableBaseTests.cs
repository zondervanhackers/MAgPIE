using System;
using Xunit;

using ZondervanLibrary.SharedLibrary.Equatable;

namespace ZondervanLibrary.SharedLibrary.Tests.Equatable
{
    internal class EquatableTest<T> : EquatableBase<EquatableTest<T>>
    {
        public T Property { get; set; }
    }

    public class EquatableBaseTests
    {

        #region Tests for Core CRL Primitive Types

        // String is the only CLR reference type with == overloaded?
        [Theory]
        [InlineData("a", "a", true)]
        [InlineData("a", "b", false)]
        [InlineData(null, "b", false)]
        [InlineData("a", null, false)]
        public void Equals_Should_Correctly_Compare_String_Properties(String propertyA, String propertyB, Boolean expected)
        {
            // Arrange
            EquatableTest<String> param1 = new EquatableTest<String>() { Property = propertyA };
            EquatableTest<String> param2 = new EquatableTest<String>() { Property = propertyB };

            // Act
            Boolean result = param1.Equals(param2);

            // Assert
            Assert.Equal(expected, result);
        }

        //[Theory]
        //[InlineData(1, 2, 1, 2, true)]
        //[InlineData(2, 1, 1, 1, true)]
        //public void Equals_Should_Correctly_Compare_IEnumerable_Properties(Int32 listValue11, Int32 listValue12, Int32 listValue21, Int32 listValue22, Boolean expected)
        //{
        //    // Arrange
        //    var param1 = new EquatableTest<IEnumerable<Int32>>() { Property = new List<Int32>() { listValue11, listValue12 } };
        //    var param2 = new EquatableTest<IEnumerable<Int32>>() { Property = new List<Int32>() { listValue21, listValue22 } };

        //    // Act
        //    var result = param1.Equals(param2);

        //    // Assert
        //    Assert.Equal(expected, result);
        //}

        #endregion
    }
}
