using System;
using Xunit;

using ZondervanLibrary.SharedLibrary.Factory;

namespace ZondervanLibrary.SharedLibrary.Tests.Factory
{
    internal class EmitDelegateTestClass
    {
        public EmitDelegateTestClass(object testReference)
        {
            TestReference = testReference;
        }

        public object TestReference { get; }
    }

    public class EmitDelegateTests
    {
        delegate Int32 Int32ConstructorDelegate();
        delegate EmitDelegateTestClass TestConstructorDelegate(object param);

        [Fact]
        public void CreateConstructorDelegate_Should_Throw_InvalidOperationException_If_No_Constructor_Matching_Parameters_Can_Be_Found()
        {
            // Arrange
            Type[] parameters = new Type[] { typeof(String) };
            
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => EmitDelegate.CreateConstructor<Int32ConstructorDelegate>());
        }

        [Fact]
        public void CreateConstructorDelegate_Should_Return_Functioning_Delegate()
        {
            // Arrange
            Type[] parameters = new Type[] { typeof(object) };
            object parameter1 = new object();
            TestConstructorDelegate constructor = EmitDelegate.CreateConstructor<TestConstructorDelegate>();

            // Act
            EmitDelegateTestClass instance = constructor(parameter1);

            // Assert
            Assert.True(ReferenceEquals(parameter1, instance.TestReference));
        }
    }
}
