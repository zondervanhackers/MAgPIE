using System;

namespace ZondervanLibrary.SharedLibrary.Equatable
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EquatablePropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a boolean determining whether this property is dependent on another property in order for it to participate in equality comparison/hash code generation.
        /// </summary>
        public String Dependency { get; set; }
        // Change to Contingency?

        /// <summary>
        /// Gets or sets whether this property is ordered or not.
        /// </summary>
        /// <remarks>
        ///     <para>This property determines whether the equality comparison and corresponding hash code are dependent upon the order of the sequence.</para>
        ///     <para>Having this property be true will result in faster equality comparisons as it results in a straightforward comparison between two sequences.</para>
        ///     <para>This property only has effect on properties implementing IEnumerable.</para>
        ///     <para>The default value is false.</para>
        /// </remarks>
        public Boolean Ordered { get; set; }

        /// <summary>
        /// Gets or sets whether this property contains a collection of unique elements.
        /// </summary>
        public Boolean Unique { get; set; }


        public Boolean Ignore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Rank { get; set; }  

        // Deal with covariance, contravariance
    }
}
