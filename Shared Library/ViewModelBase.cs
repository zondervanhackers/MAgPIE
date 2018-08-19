using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZondervanLibrary.SharedLibrary
{
    /// <summary>
    /// Provides a base implementation for all view models.
    /// </summary>
    public abstract class ViewModelBase : IViewModel
    {
        private readonly object _propertyChangedEventLock = new object();

        private PropertyChangedEventHandler _propertyChanged = delegate { };

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (_propertyChangedEventLock)
                {
                    _propertyChanged += value;
                }
            }
            remove
            {
                lock (_propertyChangedEventLock)
                {
                    _propertyChanged -= value;
                }
            }
        }

        /// <summary>
        /// Raise the PropertyChanged event.
        /// </summary>
        /// <param name="e">The arguments identifying which property has changed.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            // Copy handler to avoid threading issues
            PropertyChangedEventHandler handler;

            lock (_propertyChangedEventLock)
            {
                handler = _propertyChanged;
            }

            handler(this, e);
        }

        /// <summary>
        /// Sets <paramref name="property"/> to <paramref name="value"/> if they are not equal and fires a PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property (can be inferred automatically by compiler).</typeparam>
        /// <param name="property">A reference to the field backing the property.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property.  Do not set unless this event is being raised outside of the property definition.</param>
        protected void RaiseAndSetIfPropertyChanged<T>(ref T property, T value, [CallerMemberName] String propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(property, value))
            {
                property = value;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
