using System.Windows;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary;

using ZondervanLibrary.Harvester.Wpf.Windows;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add
{
    /// <summary>
    /// Interaction logic for RepositoryProviderDialogWindow.xaml
    /// </summary>
    public partial class AddRepositoryDialogWindow : GlassWindow, IWindow
    {
        public AddRepositoryDialogWindow(IAddRepositoryDialogViewModel viewModel, Window windowOwner)
        {
            Contract.Requires(viewModel != null);
            Contract.Requires(windowOwner != null);

            Window owner = GetWindow(windowOwner);

            Owner = owner ?? throw Argument.Exception(() => windowOwner, "{0} must be shown before it can be used.");

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
