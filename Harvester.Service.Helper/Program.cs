using System;
using System.ServiceProcess;
using System.Linq;

namespace Harvester.Service.Helper
{
    class Program
    {
        static void Main(string[] args)
        {
#if Production
                var serviceName = "Harvester Service (Production)";
#elif Test
                var serviceName = "Harvester Service (Test)";
#else
            var serviceName = "Harvester Service";
#endif

            ServiceController service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
            if (service == null) return;
            if (service.CanStop && service.Status == ServiceControllerStatus.Running)
                service.Stop();
        }
    }
}