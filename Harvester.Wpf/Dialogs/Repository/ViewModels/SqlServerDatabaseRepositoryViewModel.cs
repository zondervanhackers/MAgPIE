using System;
using System.ComponentModel;

using ZondervanLibrary.SharedLibrary;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels
{
    public enum SqlServerAuthenticationMethod
    {
        Integrated,
        [Description("SQL Server")]
        SqlServer
    }

    public class SqlServerDatabaseRepositoryViewModel : ViewModelBase, IDatabaseRepositoryViewModel
    {
        public SqlServerDatabaseRepositoryViewModel()
        {
            // Integrated security is more secure and thus should be the default.
            AuthenticationMethod = SqlServerAuthenticationMethod.Integrated;
        }

        public String Name => "SQL Server";

        public String Description => "SQL Server Database";

        private String _server;
        public String Server
        {
            get => _server;
            set => RaiseAndSetIfPropertyChanged(ref _server, value);
        }

        private String _database;
        public String Database
        {
            get => _database;
            set => RaiseAndSetIfPropertyChanged(ref _database, value);
        }
        
        private SqlServerAuthenticationMethod _authenticationMethod;
        public SqlServerAuthenticationMethod AuthenticationMethod
        {
            get => _authenticationMethod;
            set => RaiseAndSetIfPropertyChanged(ref _authenticationMethod, value);
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
    }
}
