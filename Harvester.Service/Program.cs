using System;
using System.Collections;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace ZondervanLibrary.Harvester.Service
{
    public static class Program
    {

#if Production
        static string serviceName = "Harvester Service (Production)";
#elif Test
        static string serviceName = "Harvester Service (Test)";
#else
        static string serviceName = serviceName;
#endif
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            string parameter = string.Concat(args);

            ServiceController service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
            if (args.Length == 0)
            {
                ServiceBase[] servicesToRun = { new HarvesterService() };
                ServiceBase.Run(servicesToRun);
            }
            else
            {
                switch (parameter)
                {
                    case "-install":
                        if (service != null)
                        {
                            StopService();
                            UninstallService();
                        }

                        InstallService();
                        StartService();
                        break;
                    case "-uninstall":
                        StopService();
                        UninstallService();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static void InstallService()
        {
            if (IsInstalled()) return;

            using (AssemblyInstaller installer = GetInstaller())
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch
                    {
                        // ignored
                    }

                    throw;
                }
            }
        }

        private static void UninstallService()
        {
            if (!IsInstalled()) return;
            using (AssemblyInstaller installer = GetInstaller())
            {
                IDictionary state = new Hashtable();
                installer.Uninstall(state);
            }
        }

        private static void StartService()
        {
            if (!IsInstalled()) return;

            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }
            }
        }

        private static void StopService()
        {
            if (!IsRunning())
                return;

            using (ServiceController controller = new ServiceController(serviceName))
            {
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
        }

        private static bool IsInstalled()
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private static bool IsRunning()
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (!IsInstalled())
                    return false;

                return controller.Status == ServiceControllerStatus.Running;
            }
        }

        private static AssemblyInstaller GetInstaller()
        {
            return new AssemblyInstaller(typeof(HarvesterService).Assembly, null) { UseNewContext = true };
        }
    }
}