using System;
using System.Net;
using System.ServiceProcess;

namespace Thriot.Platform.TelemetryQueueService
{
    static class Program
    {
        static void Main()
        {
            SetupNetworking();

            if (Environment.UserInteractive)
            {
                new ConsoleRunner().Run();
            }
            else
            {
                var servicesToRun = new ServiceBase[]
                {
                    new TelemetryQueueServiceInstance()
                };
                ServiceBase.Run(servicesToRun);
            }
        }

        private static void SetupNetworking()
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }
    }
}
