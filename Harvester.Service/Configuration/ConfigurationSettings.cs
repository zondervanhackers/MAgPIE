using ZondervanLibrary.Harvester.Core.Operations;
using ZondervanLibrary.Harvester.Core.Repository;

namespace ZondervanLibrary.Harvester.Service.Configuration
{
    public class ConfigurationSettings
    {
        public NotificationSettings Notification { get; set; }

        public RepositoryArgumentsBase[] Repositories { get; set; }
        
        public OperationArgumentsBase[] Operations { get; set; }
    }
}
