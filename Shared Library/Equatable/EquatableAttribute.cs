using System;

namespace ZondervanLibrary.SharedLibrary.Equatable
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EquatableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets whether the class is immutable.
        /// </summary>
        /// <remarks>
        ///     <para>If set to true the hash code for the object will be cached permanently the first time it is computed.</para>
        ///     <para>The default value is false.</para>
        /// </remarks>
        public Boolean IsImmutable { get; set; }

        /// <summary>
        /// Gets or sets whether public properties require the EquatableProperty attribute to participate the equality comparison.
        /// </summary>
        /// <remarks>
        ///     <para>The default value is false.</para>
        /// </remarks>
        public Boolean ExplicitMode { get; set; }
    }
}
