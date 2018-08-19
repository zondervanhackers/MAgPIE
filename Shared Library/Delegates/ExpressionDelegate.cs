using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace ZondervanLibrary.SharedLibrary.Delegates
{
    /// <summary>
    /// A delegate that expands 
    /// </summary>
    /// <param name="args">doc</param>
    public delegate void ArgumentExpansionDelegate(params object[] args);

    /// <summary>
    /// Doc
    /// </summary>
    public static class ExpressionDelegate
    {
        /// <summary>
        /// Doc
        /// </summary>
        /// <param name="method">doc</param>
        /// <param name="parameters">doc</param>
        /// <returns>doc</returns>
        public static ArgumentExpansionDelegate CreateDelegate(object instance, MethodInfo method, Type[] parameters)
        {
            Expression instanceExpression = Expression.Constant(instance);

            ParameterExpression args = Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExpression = parameters.Select((type, index) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(index)), type)).ToArray();

            Expression callExpression = Expression.Call(instanceExpression, method, argsExpression);

            LambdaExpression lambda = Expression.Lambda(typeof(ArgumentExpansionDelegate), callExpression, args);

            return (ArgumentExpansionDelegate)lambda.Compile();
        }
    }
}
