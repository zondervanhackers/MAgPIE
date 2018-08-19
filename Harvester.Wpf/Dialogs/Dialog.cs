using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary.Factory;

using ZondervanLibrary.Harvester.Wpf.Windows;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs
{
    public class Dialog<TViewModel> : IDialog
    {
        private readonly IFactory<TViewModel> _viewModelFactory;
        private readonly IFactory<IWindow, TViewModel> _windowFactory;

        public Dialog(IFactory<TViewModel> viewModelFactory, IFactory<IWindow, TViewModel> windowFactory)
        {
            Contract.Requires(viewModelFactory != null);
            Contract.Requires(windowFactory != null);

            _viewModelFactory = viewModelFactory;
            _windowFactory = windowFactory;
        }

        public Boolean ShowDialog()
        {
            TViewModel viewModel = _viewModelFactory.CreateInstance();

            IWindow window = _windowFactory.CreateInstance(viewModel);

            Boolean? result = window.ShowDialog();

            Contract.Assert(result != null);

            return (Boolean)result;
        }
    }
}