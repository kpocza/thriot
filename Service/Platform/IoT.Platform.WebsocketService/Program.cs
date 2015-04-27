using System;
using System.Net;
using System.ServiceProcess;

namespace Thriot.Platform.WebsocketService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            SetupNetworking();

            if (Environment.UserInteractive)
            {
                new ConsoleRunner().Run();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new WebSocketServiceInstance() 
                };
                ServiceBase.Run(ServicesToRun);
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
