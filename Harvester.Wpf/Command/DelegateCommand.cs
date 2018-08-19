using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Data;
using System.Reflection;

using ZondervanLibrary.SharedLibrary.Delegates;

namespace ZondervanLibrary.Harvester.Wpf.Command
{
    [ContentProperty("Arguments")]
    public class DelegateCommand : Freezable, ICommand
    {
        private ArgumentExpansionDelegate _executeDelegate;

        private EventHandler _canExecuteChanged;
        private readonly object _canExecuteChangedLock = new object();

        public DelegateCommand()
        {
            // Bind InheritedDataContextProperty with a default binding which will return the DataContext of DelegateCommand
            BindingOperations.SetBinding(this, InheritedDataContextProperty, new Binding());

            Arguments = new FreezableCollection<DelegateArgument>();
        }

        #region Freezable Overrides

        protected override Freezable CreateInstanceCore()
        {
            return new DelegateCommand();
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Blah blah blah
        /// </summary>
        public Boolean CanExecute
        {
            get => (Boolean)GetValue(CanExecuteProperty);
            set => SetValue(CanExecuteProperty, value);
        }

        /// <summary>
        ///  BLah
        /// </summary>
        public static readonly DependencyProperty CanExecuteProperty = DependencyProperty.Register("CanExecute", typeof(Boolean), typeof(DelegateCommand), new PropertyMetadata(true, CanExecuteChanged));

        private static void CanExecuteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelegateCommand command = d as DelegateCommand;

            command?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Blah blah blah
        /// </summary>
        public String Execute
        {
            get => (String)GetValue(ExecuteProperty);
            set => SetValue(ExecuteProperty, value);
        }

        /// <summary>
        /// Proper
        /// </summary>
        public static readonly DependencyProperty ExecuteProperty = DependencyProperty.Register("Execute", typeof(String), typeof(DelegateCommand), new PropertyMetadata(ExecuteChanged));

        private static void ExecuteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DelegateCommand command)
            {
                //command.CreateBinding();
            }
        }


        private object InheritedDataContext
        {
            get => GetValue(InheritedDataContextProperty);
            set => SetValue(InheritedDataContextProperty, value);
        }

        private static readonly DependencyProperty InheritedDataContextProperty = DependencyProperty.Register("InheritedDataContext", typeof(object), typeof(DelegateCommand));

        /// <summary>
        /// Docs
        /// </summary>
        public object Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        /// <summary>
        /// Docs
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(DelegateCommand), new PropertyMetadata(null, TargetChanged));

        private static void TargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DelegateCommand command)
            {
                //command.CreateBinding();
            }
        }

        public FreezableCollection<DelegateArgument> Arguments
        {
            get => (FreezableCollection<DelegateArgument>)GetValue(ArgumentsProperty);
            set => SetValue(ArgumentsProperty, value);
        }

        public static readonly DependencyProperty ArgumentsProperty = DependencyProperty.Register("Arguments", typeof(FreezableCollection<DelegateArgument>), typeof(DelegateCommand));

        #endregion

        #region ICommand Implementation

        private void GenerateDelegate(object parameter)
        {
            object invocationTarget = Target ?? InheritedDataContext;

            if (invocationTarget == null)
            {
                throw new CommandBindingException("A target for Execute must be set explicity as this item.\nAn inherited data context could not be found.", null, Execute);
            }

            if (Execute == null)
            {
                throw new CommandBindingException("Execute must be specified.", invocationTarget.GetType());
            }

            Type[] argumentTypes = Arguments.Select(argument => {
                if (argument.Type != null)
                {
                    return argument.Type;
                }
                else if (argument.IsCommandParameter)
                {
                    if (parameter == null)
                        throw new CommandBindingException("An argument's IsCommandParameter cannot be true if no Command Parameter is specified.", invocationTarget.GetType(), Execute);

                    return parameter.GetType();
                }
                else
                {
                    if (argument.Value == null)
                        throw new CommandBindingException("The value for an argument cannot be null.", invocationTarget.GetType(), Execute);

                    return argument.Value.GetType();
                }
            }).ToArray();

            MethodInfo method = invocationTarget.GetType().GetMethod(Execute, argumentTypes);

            if (method == null)
            {
                throw new CommandBindingException("The specified method could not be found on the invocation target.", invocationTarget.GetType(), Execute, argumentTypes);
            }

            try
            {
                _executeDelegate = ExpressionDelegate.CreateDelegate(invocationTarget, method, argumentTypes);
            }
            catch (Exception innerException)
            {
                throw new CommandBindingException("Could not create delegate to method on the invocation target.", innerException, invocationTarget.GetType(), Execute, argumentTypes);
            }
        }

        /// <inheritdoc/>
        Boolean ICommand.CanExecute(object parameter)
        {
            return CanExecute;
        }

        /// <inheritdoc/>
        void ICommand.Execute(object parameter)
        {
            if (_executeDelegate == null)
                GenerateDelegate(parameter);

            object[] arguments = Arguments.Select(argument => argument.IsCommandParameter ? parameter : argument.Value).ToArray();

            _executeDelegate(arguments);
        }

        /// <inheritdoc/>
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                lock (_canExecuteChangedLock)
                {
                    _canExecuteChanged += value;
                }
            }
            remove
            {
                lock (_canExecuteChangedLock)
                {
                    _canExecuteChanged -= value;
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Docs
        /// </summary>
        /// <param name="e">Docs</param>
        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            EventHandler handler;

            lock (_canExecuteChangedLock)
            {
                handler = _canExecuteChanged;
            }

            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Docs
        /// </summary>
        protected void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(new EventArgs());
        }

        #endregion
    }
}
