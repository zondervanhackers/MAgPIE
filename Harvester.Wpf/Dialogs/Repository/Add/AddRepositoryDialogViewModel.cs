using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add
{
    public class AddRepositoryDialogViewModel : ViewModelBase, IAddRepositoryDialogViewModel
    {
        public AddRepositoryDialogViewModel(IEnumerable<IRepositoryGroupViewModel> repositoryGroups)
        {
            Contract.Requires(repositoryGroups != null);
            Contract.Requires(repositoryGroups.Any());

            RepositoryGroups = repositoryGroups;
        }

        public IEnumerable<IRepositoryGroupViewModel> RepositoryGroups { get; }

        private IRepositoryGroupViewModel _selectedRepositoryGroup;
        public IRepositoryGroupViewModel SelectedRepositoryGroup
        {
            get => _selectedRepositoryGroup;
            set => RaiseAndSetIfPropertyChanged(ref _selectedRepositoryGroup, value);
        }
    }
} 
