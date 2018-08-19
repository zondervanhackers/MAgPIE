using System;
using Xunit;

using ZondervanLibrary.SharedLibrary.Binding;


namespace ZondervanLibrary.SharedLibrary.Tests.Binding
{
    public class BindingExtensionsTests
    {
        [Fact]
        public void Bind_Should_Not_Apply_Action_If_Object_Is_Null()
        {
            // Arrange
            String obj = null;
            Boolean actionCalled = false;
   
            // Act
            obj.Bind(s => { actionCalled = true; });

            // Assert
            Assert.False(actionCalled);
        }

        [Fact]
        public void Bind_Should_Not_Apply_Action_If_Nullable_Is_Null()
        {
            // Arrange
            Int32? obj = null;
            Boolean actionCalled = false;
            //Action<int> action = s => { actionCalled = true; };

            // Act
            //obj.Bind(s => { actionCalled = true; });

            // Assert
            Assert.False(actionCalled);
        }

        [Fact]
        public void Bind1_Should_Apply_Action_If_Object_Is_Not_Null()
        {
            // Arrange
            String obj = "asdf";
            Boolean actionCalled = false;

            void Action(String s)
            {
                actionCalled = true;
            }

            // Act
            obj.Bind(Action);

            // Assert
            Assert.True(actionCalled);
        }

        [Fact]
        public void Bind2_Should_Return_Null_If_Object_Is_Null()
        {
            // Arrange
            String obj = null;
            String Func(String s) => s.ToUpper();

            // Act
            String result = obj.Bind(Func);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Bind_Should_Return_Default_If_Nullable_Is_Null()
        {
            // Arrange
            Int32? obj = null;

            // Act
            //var result = obj.Bind(o => o + 1);
        }

        [Fact]
        public void Bind2_Should_Return_Function_Result_If_Object_Is_Not_Null()
        {
            // Arrange
            String obj = "a";
            String Func(String s) => s.ToUpper();

            // Act
            String result = obj.Bind(Func);

            // Assert
            Assert.Equal(obj.ToUpper(), result);
        }
    }
}
