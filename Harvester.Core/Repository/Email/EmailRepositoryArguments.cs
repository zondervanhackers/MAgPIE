using ZondervanLibrary.Harvester.Core.Repository.Directory;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public class EmailRepositoryArguments : RepositoryArgumentsBase
    {
        public string HostName;

        public int Port;

        public string Username;

        public string Password;

        public bool UseSsl;

        public string FromAddress;

        public FolderDirectoryRepositoryArguments JsonRepository { get; set; }
    }
}
