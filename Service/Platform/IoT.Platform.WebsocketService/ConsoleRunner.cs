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

            _brain = new Brain();
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
