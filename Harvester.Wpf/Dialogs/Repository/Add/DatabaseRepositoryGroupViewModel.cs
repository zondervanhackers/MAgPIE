using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary;

using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add
{
    public class DatabaseRepositoryGroupViewModel : ViewModelBase, IRepositoryGroupViewModel
    {
        private readonly IEnumerable<IDatabaseRepositoryViewModel> _repositories;

        public DatabaseRepositoryGroupViewModel(IEnumerable<IDatabaseRepositoryViewModel> repositories)
        {
            Contract.Requires(repositories != null);
            Contract.Requires(repositories.Any());

            _repositories = repositories;

            SelectedRepository = repositories.First();
        }

        public String Name => "Database";

        public IEnumerable<IRepositoryViewModel> Repositories => _repositories;

        private IRepositoryViewModel _selectedRepository;
        public IRepositoryViewModel SelectedRepository
        {
            get => _selectedRepository;
            set => RaiseAndSetIfPropertyChanged(ref _selectedRepository, value);
        }
    }
}
