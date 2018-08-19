using System;
using System.Windows;
using System.Windows.Input;

namespace ZondervanLibrary.Harvester.Wpf.Command
{
    /// <summary>
    /// Docs
    /// </summary>
    public class DelegateArgument : Freezable
    {
        /// <summary>
        /// Creates a new instance of <see cref="DelegateArgument"/>.
        /// </summary>
        public DelegateArgument()
        {
            IsCommandParameter = false;
            Type = null;
        }

        #region Freezable Overrides

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new DelegateArgument();
        }

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Gets or sets the type of this argument.
        /// </summary>
        /// <remarks>
        ///     <para>This value only needs to be set only if the type cannot be determined from the value of <see cref="Value"/> or <see cref="ICommandSource.CommandParameter"/>.</para>
        /// </remarks>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the value of this argument.
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// The <see cref="DependencyProperty"/> backing for <see cref="Value"/>.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DelegateArgument));

        /// <summary>
        /// Gets or sets whether this argument should be set to the value of the passed <see cref="ICommandSource.CommandParameter"/>.
        /// </summary>
        public Boolean IsCommandParameter { get; set; }

        #endregion
    }
}
