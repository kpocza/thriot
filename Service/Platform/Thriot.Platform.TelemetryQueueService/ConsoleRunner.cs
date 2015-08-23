using System;
using System.Runtime.InteropServices;
using Microsoft.Framework.DependencyInjection;
using Thriot.Platform.Services.Telemetry.Recording;

namespace Thriot.Platform.TelemetryQueueService
{
    class ConsoleRunner
    {
        internal void Run()
        {
            AllocConsole();

            var servicesSetup = new ServicesSetup();
            servicesSetup.Setup();

            var serviceProvider = servicesSetup.GetServiceProvider();
            var queueProcessor = serviceProvider.GetService<QueueProcessor>();

            queueProcessor.Start();

            Console.WriteLine("Started");
            Console.ReadLine();

            queueProcessor.Stop();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
