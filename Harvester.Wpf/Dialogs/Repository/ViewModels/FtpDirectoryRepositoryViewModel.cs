using System;
using ZondervanLibrary.SharedLibrary;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels
{
    public class FtpDirectoryRepositoryViewModel : ViewModelBase, IDirectoryRepositoryViewModel
    {
        public FtpDirectoryRepositoryViewModel()
        {
            // Default should be to use SSL
            UseSsl = true;
        }

        public String Name => "FTP";

        public String Description => "File Transfer Protocol";

        private String _host;
        public String Host
        {
            get => _host;
            set => RaiseAndSetIfPropertyChanged(ref _host, value);
        }

        private Int32 _port;
        public Int32 Port
        {
            get => _port;
            set => RaiseAndSetIfPropertyChanged(ref _port, value);
        }

        private String _username;
        public String Username
        {
            get => _username;
            set => RaiseAndSetIfPropertyChanged(ref _username, value);
        }

        private String _password;
        public String Password
        {
            get => _password;
            set => RaiseAndSetIfPropertyChanged(ref _password, value);
        }

        private Boolean _useSsl;
        public Boolean UseSsl
        {
            get => _useSsl;
            set => RaiseAndSetIfPropertyChanged(ref _useSsl, value);
        }
    }
}
