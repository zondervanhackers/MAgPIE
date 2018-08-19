using System;
using ZondervanLibrary.SharedLibrary;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels
{
    public class FolderDirectoryRepositoryViewModel : ViewModelBase, IDirectoryRepositoryViewModel
    {
        public String Name => "Folder";

        public String Description => "Folder";

        private String _directoryPath;
        public String DirectoryPath
        {
            get => _directoryPath;
            set => RaiseAndSetIfPropertyChanged(ref _directoryPath, value);
        }


    }
}
