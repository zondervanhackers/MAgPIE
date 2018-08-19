using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

using ZondervanLibrary.SharedLibrary;

using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add
{
    public class JournalRepositoryGroupViewModel : ViewModelBase, IRepositoryGroupViewModel
    {
        public JournalRepositoryGroupViewModel(IEnumerable<IJournalRepositoryViewModel> repositories)
        {
            Contract.Requires(repositories != null);
            Contract.Requires(repositories.Any());

            Repositories = repositories;

            SelectedRepository = repositories.First();
        }

        public String Name => "Journal";

        public IEnumerable<IRepositoryViewModel> Repositories { get; }

        private IRepositoryViewModel _selectedRepository;
        public IRepositoryViewModel SelectedRepository
        {
            get => _selectedRepository;
            set => RaiseAndSetIfPropertyChanged(ref _selectedRepository, value);
        }
    }
}
