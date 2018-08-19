using System;

namespace ZondervanLibrary.SharedLibrary.Binding
{
    public static class BindingExtensions
    {
        /// <summary>
        /// Binds an action to an object that is only applied if the object is not null.
        /// </summary>
        /// <typeparam name="T">The type of the underlying object.</typeparam>
        /// <param name="obj">The nullable object.</param>
        /// <param name="action">The action to perform on the object when it is not null.</param>
        public static void Bind<T>(this T obj, Action<T> action)
            where T : class
        {
            if (obj != null)
                action(obj);
        }

        /// <summary>
        /// Binds an action to an object that is only applied if the object is not null.
        /// </summary>
        /// <typeparam name="T">The type of the object wrapped in the Nullable object.</typeparam>
        /// <param name="obj">The nullable object.</param>
        /// <param name="action">The action to perform on the object.</param>
        public static void Bind<T>(this T? obj, Action<T> action)
            where T : struct
        {
            if (obj.HasValue)
                action(obj.Value);
        }

        /// <summary>
        /// Binds an function to an object that is only applied if the object is not null.
        /// </summary>
        /// <typeparam name="T">The type of the underlying object.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The nullable object.</param>
        /// <param name="func">The function to perform on the object when it is not null.</param>
        /// <returns>Either null if obj is null or the result of applying obj to the function.</returns>
        public static TResult Bind<T, TResult>(this T obj, Func<T, TResult> func, TResult @default = default(TResult))
            where T : class
        {
            return obj == null ? @default : func(obj);
        }

        /// <summary>
        /// Binds an function to an object that is only applied if the object is not null.
        /// </summary>
        /// <typeparam name="T">The type of the object wrapped in a nullable.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The nullable object.</param>
        /// <param name="func">The function to perform on the object when it is not null.</param>
        /// <param name="@default">The result to return when obj is null.</param>
        /// <returns>Either @default if obj is null or the result of applying obj to the function.</returns>
        public static TResult Bind<T, TResult>(this T? obj, Func<T, TResult> func, TResult @default = default(TResult))
            where T : struct
        {
            return obj.HasValue ? func(obj.Value) : @default;
        }
    }
}