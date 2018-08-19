using System;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary.Factory;

using ZondervanLibrary.Harvester.Wpf.Windows;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add
{
    public class AddRepositoryDialog : IAddRepositoryDialog
    {
        private readonly IFactory<IAddRepositoryDialogViewModel> _viewModelFactory;
        private readonly IFactory<IWindow, IAddRepositoryDialogViewModel> _windowFactory;

        public AddRepositoryDialog(IFactory<IAddRepositoryDialogViewModel> viewModelFactory, IFactory<IWindow, IAddRepositoryDialogViewModel> windowFactory)
        {
            Contract.Requires(viewModelFactory != null);
            Contract.Requires(windowFactory != null);

            _viewModelFactory = viewModelFactory;
            _windowFactory = windowFactory;
        }

        /// <inheritdoc/>
        public Boolean ShowDialog()
        {
            IAddRepositoryDialogViewModel viewModel = _viewModelFactory.CreateInstance();

            IWindow window = _windowFactory.CreateInstance(viewModel);

            Boolean? result = window.ShowDialog();

            Contract.Assert(result != null);

            return (Boolean)result;
        }
    }
}
