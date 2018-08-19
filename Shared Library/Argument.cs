using System;
using System.Linq.Expressions;

namespace ZondervanLibrary.SharedLibrary
{
    /// <summary>
    /// Contains static methods to create instances of <see cref="System.ArgumentException"/> without using "magic" argument names.
    /// </summary>
    /// <remarks>
    ///     <para>Argument is designed to allow for throwing <see cref="System.ArgumentException"/> and its derivates (<see cref="System.ArgumentNullException"/> and <see cref="System.ArgumentOutOfRangeException"/>) in a way that captures the name of a parameter using an <see cref="System.Linq.Expressions.Expression"/> instead of a hardcoded <see cref="System.String"/>.  This ensures that the name will be updated if the parameter name is refactored.</para>
    ///     <para>The nomenclature of the methods in Argument is designed to be analagous to manually throwing <see cref="System.ArgumentException"/>.</para>
    ///     <para>Compare<code language="C#">throw new ArgumentException("paramName");</code> to <code language="C#">throw Argument.Exception(() => paramName);</code></para>
    ///     <para>Inspiration for this idea is due to the blog post "'Magic' null argument testing" by Jon Skeet (<see href="https://msmvps.com/blogs/jon_skeet/archive/2009/12/09/quot-magic-quot-null-argument-testing.aspx"/>).</para>
    /// </remarks>
    /// <seealso cref="System.ArgumentException"/>
    /// <seealso cref="System.ArgumentNullException"/>
    /// <seealso cref="System.ArgumentOutOfRangeException"/>
    /// <seealso cref="System.Linq.Expressions.Expression"/>
    public static class Argument
    {
        /// <summary>
        /// Returns an <see cref="System.ArgumentException"/> with the specified parameter name.
        /// </summary>
        /// <param name="expression">The expression encapsulating the argument (see the code example).</param>
        /// <typeparam name="T">The type of the argument that expression captures.</typeparam>
        /// <returns>An <see cref="System.ArgumentException"/> with <see cref="System.ArgumentException.ParamName">ArgumentException.ParamName</see> set to the name of the argument.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Using this function to create instances of <see cref="System.ArgumentException"/> makes code refactoring much simpler as the argument name is not hardcoded as a <see cref="System.String"/>.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(String argument)
        ///             {
        ///                 if (argument == "")
        ///                     throw Argument.Exception(() => argument);
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// <seealso cref="System.ArgumentException"/>
        /// <seealso cref="System.Linq.Expressions.Expression"/>
        public static ArgumentException Exception<T>(Expression<Func<T>> expression)
        {
            return Exception(expression, null, null);
        }

        //Failing Line;

        /// <summary>Returns an <see cref="System.ArgumentException"/> with the specified parameter name and message.</summary>
        /// <param name="expression">The expression encapsulating the argument (see the code example).</param>
        /// <param name="message">The message describing the exception.</param>
        /// <typeparam name="T">The type of the argument that expression captures.</typeparam>
        /// <returns>An <see cref="System.ArgumentException"/> with <see cref="System.ArgumentException.ParamName">ArgumentException.ParamName</see> set to the name of the argument and <see cref="System.ArgumentException.Message">ArgumentException.Message</see> set to <paramref name="message"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Using this function to create instances of <see cref="System.ArgumentException"/> makes code refactoring much simpler as the argument name is not hardcoded as a <see cref="System.String"/>.</para>
        ///     <para>You may insert <c>{0}</c> into the message and it will be replaced by the name of the argument.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(String argument)
        ///             {
        ///                 if (argument == "")
        ///                     throw Argument.Exception(() => argument, "{0} cannot be an empty string.");
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// <seealso cref="System.ArgumentException"/>
        /// <seealso cref="System.Linq.Expressions.Expression"/>
        public static ArgumentException Exception<T>(Expression<Func<T>> expression, String message)
        {
            return Exception(expression, message, null);
        }

        /// <summary>Returns an <see cref="System.ArgumentException"/> with the specified parameter name, message, and inner exception.</summary>
        /// <param name="expression">The expression encapsulating the argument (see the code example).</param>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The innerException that caused this exception to be thrown.</param>
        /// <typeparam name="T">The type of the argument that expression captures.</typeparam>
        /// <returns>An <see cref="System.ArgumentException"/> with <see cref="System.ArgumentException.ParamName">ArgumentException.ParamName</see> set to the name of the argument, <see cref="System.ArgumentException.Message">ArgumentException.Message</see> set to <paramref name="message"/>, and <see cref="System.Exception.InnerException">ArgumentException.InnerException</see> set to <paramref name="innerException"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Using this function to create instances of <see cref="System.ArgumentException"/> makes code refactoring much simpler as the argument name is not hardcoded as a <see cref="System.String"/>.</para>
        ///     <para>You may specify insert <c>{0}</c> into the message and it will be replaced by the name of the argument.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(String fileName)
        ///             {
        ///                 try
        ///                 {
        ///                     System.IO.File.Open(fileName, FileMode.Open);
        ///                 }
        ///                 catch (Exception ex)
        ///                 {
        ///                     throw Argument.Exception(() => argument, "{0} cannot be an empty string.", ex);
        ///                 }
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// <seealso cref="System.ArgumentException"/>
        /// <seealso cref="System.Linq.Expressions.Expression"/>
        public static ArgumentException Exception<T>(Expression<Func<T>> expression, String message, Exception innerException)
        {
            String argumentName = NameOf(expression);
            String finalMessage = (message == null) ? null : String.Format(message, argumentName);

            return new ArgumentException(finalMessage, argumentName, innerException);
        }

        /// <summary>
        /// Returns an <see cref="System.ArgumentNullException"/> with the specified parameter name.
        /// </summary>
        /// <param name="expression">The expression encapsulating the argument (see the code example).</param>
        /// <typeparam name="T">The type of the argument that expression captures.</typeparam>
        /// <returns>An <see cref="System.ArgumentNullException"/> with <see cref="System.ArgumentException.ParamName">ArgumentNullException.ParamName</see> set to the name of the argument.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Using this function to create instances of <see cref="System.ArgumentNullException"/> makes code refactoring much simpler as the argument name is not hardcoded as a <see cref="System.String"/>.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(String argument)
        ///             {
        ///                 if (argument == null)
        ///                     throw Argument.NullException(() => argument);
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// <seealso cref="System.ArgumentNullException"/>
        /// <seealso cref="System.Linq.Expressions.Expression"/>
        public static ArgumentNullException NullException<T>(Expression<Func<T>> expression)
        {
            return NullException(expression, null);
        }

        /// <summary>
        /// Returns an <see cref="System.ArgumentNullException"/> with the specified parameter name and message.
        /// </summary>
        /// <param name="expression">The expression encapsulating the argument (see the code example).</param>
        /// <param name="message">The message describing the exception.</param>
        /// <typeparam name="T">The type of the argument that expression captures.</typeparam>
        /// <returns>An <see cref="System.ArgumentNullException"/> with <see cref="System.ArgumentException.ParamName">ArgumentNullException.ParamName</see> set to the name of the argument and <see cref="System.ArgumentException.Message">ArgumentNullException.Message</see> set to <paramref name="message"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Using this function to create instances of <see cref="System.ArgumentNullException"/> makes code refactoring much simpler as the argument name is not hardcoded as a <see cref="System.String"/>.</para>
        ///     <para>You may specify insert <c>{0}</c> into the message and it will be replaced by the name of the argument.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(String argument)
        ///             {
        ///                 if (argument == null)
        ///                     throw Argument.NullException(() => argument, "{0} cannot be null.");
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// <seealso cref="System.ArgumentNullException"/>
        /// <seealso cref="System.Linq.Expressions.Expression"/>
        public static ArgumentNullException NullException<T>(Expression<Func<T>> expression, String message)
        {
            String argumentName = NameOf(expression);

            if (message == null)
            {
                return new ArgumentNullException(argumentName);
            }
            else
            {
                return new ArgumentNullException(argumentName, String.Format(message, argumentName));
            }
        }

        /// <summary>
        /// Returns an <see cref="System.ArgumentOutOfRangeException"/> with the specified parameter name.
        /// </summary>
        /// <param name="expression">The expression encapsulating the argument (see the code example).</param>
        /// <typeparam name="T">The type of the argument that expression captures.</typeparam>
        /// <returns>An <see cref="System.ArgumentOutOfRangeException"/> with <see cref="System.ArgumentException.ParamName">ArgumentNullException.ParamName</see> set to the name of the argument.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Using this function to create instances of <see cref="System.ArgumentOutOfRangeException"/> makes code refactoring much simpler as the argument name is not hardcoded as a <see cref="System.String"/>.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(Int32 argument)
        ///             {
        ///                 if (argument &lt; 0)
        ///                     throw Argument.OutOfRangeException(() => argument);
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// <seealso cref="System.ArgumentOutOfRangeException"/>
        /// <seealso cref="System.Linq.Expressions.Expression"/>
        public static ArgumentOutOfRangeException OutOfRangeException<T>(Expression<Func<T>> expression)
        {
            return OutOfRangeException(expression, null);
        }

        /// <summary>
        /// Returns an <see cref="System.ArgumentOutOfRangeException"/> with the specified parameter name and message.
        /// </summary>
        /// <param name="expression">The expression encapsulating the argument (see the code example).</param>
        /// <param name="message">The message describing the exception.</param>
        /// <typeparam name="T">The type of the argument that expression captures.</typeparam>
        /// <returns>An <see cref="System.ArgumentOutOfRangeException"/> with <see cref="System.ArgumentException.ParamName">ArgumentNullException.ParamName</see> set to the name of the argument and <see cref="System.ArgumentOutOfRangeException.Message">ArgumentNullException.Message</see> set to <paramref name="message"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Using this function to create instances of <see cref="System.ArgumentOutOfRangeException"/> makes code refactoring much simpler as the argument name is not hardcoded as a <see cref="System.String"/>.</para>
        ///     <para>You may specify insert <c>{0}</c> into the message and it will be replaced by the name of the argument.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(Int32 argument)
        ///             {
        ///                 if (argument &lt; 0)
        ///                     throw Argument.OutOfRangeException(() => argument, "{0} cannot be negative.");
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// <seealso cref="System.ArgumentOutOfRangeException"/>
        /// <seealso cref="System.Linq.Expressions.Expression"/>
        public static ArgumentOutOfRangeException OutOfRangeException<T>(Expression<Func<T>> expression, String message)
        {
            String argumentName = NameOf(expression);

            if (message == null)
            {
                return new ArgumentOutOfRangeException(argumentName);
            }
            else
            {
                return new ArgumentOutOfRangeException(argumentName, String.Format(message, argumentName));
            }
        }

        /// <summary>
        /// Returns the name of the member from the passed expression.
        /// </summary>
        /// <typeparam name="T">The type of the member captured by expression.</typeparam>
        /// <param name="expression">The expression capturing the member name.</param>
        /// <returns>The name of the member captured by <paramref name="expression"/></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="expression"/> does not contain a <see cref="System.Linq.Expressions.MemberExpression"/> as its body.</exception>
        /// <remarks>
        ///     <para>Use this function to create a reference to the name of a parameter.  This makes code refactoring much simpler as Visual Studio can update references automatically.</para>
        /// </remarks>
        /// <example>
        ///     <code language="C#">
        ///         public class Example
        ///         {
        ///             public void ExampleFunction(String argument)
        ///             {
        ///                 System.Console.WriteLine(String.Format("The name of the parameter is {0}", Argument.NameOf(() => argument)));
        ///             }
        ///         }
        ///     </code>
        /// </example>
        public static String NameOf<T>(Expression<Func<T>> expression)
        {
            if (!(expression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentOutOfRangeException(nameof(expression), "expression must be a MemberExpression");
            }

            return memberExpression.Member.Name;
        }
    }
}
