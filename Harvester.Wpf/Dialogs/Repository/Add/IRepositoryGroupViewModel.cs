using System;
using System.Collections.Generic;
using ZondervanLibrary.SharedLibrary;

using ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.Add
{
    public interface IRepositoryGroupViewModel : IViewModel
    {
        String Name { get; }

        IEnumerable<IRepositoryViewModel> Repositories { get; }

        IRepositoryViewModel SelectedRepository { get; set; }
    }
}
