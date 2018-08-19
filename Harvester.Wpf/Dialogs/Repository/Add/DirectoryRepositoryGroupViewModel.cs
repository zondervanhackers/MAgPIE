using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary;

using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add
{
    public class DirectoryRepositoryGroupViewModel : ViewModelBase, IRepositoryGroupViewModel
    {
        private readonly IEnumerable<IDirectoryRepositoryViewModel> _repositories;

        public DirectoryRepositoryGroupViewModel(IEnumerable<IDirectoryRepositoryViewModel> repositories)
        {
            Contract.Requires(repositories != null);
            Contract.Requires(repositories.Any());

            _repositories = repositories;

            SelectedRepository = repositories.First();
        }

        public String Name => "Directory";

        public IEnumerable<IRepositoryViewModel> Repositories => _repositories;

        private IRepositoryViewModel _selectedRepository;
        public IRepositoryViewModel SelectedRepository
        {
            get => _selectedRepository;
            set => RaiseAndSetIfPropertyChanged(ref _selectedRepository, value);
        }
    }
}
