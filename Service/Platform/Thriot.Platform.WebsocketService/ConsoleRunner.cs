using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Thriot.Platform.WebsocketService
{
    class ConsoleRunner
    {
        private Brain _brain;

        internal void Run()
        {
            AllocConsole();

            var serviceSetup = new ServicesSetup();
            serviceSetup.Setup();

            _brain = new Brain(serviceSetup.GetServiceProvider());
            _brain.Start();

            Console.WriteLine("Started");
            Console.ReadLine();

            _brain.Stop();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
