using System.Net;
using System.ServiceProcess;
using Thriot.Framework;
using Environment = System.Environment;

namespace Thriot.Platform.WebsocketService
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
                    new WebSocketServiceInstance() 
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
