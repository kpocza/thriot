﻿using System;
using System.Runtime.InteropServices;
using Thriot.Framework;

namespace Thriot.Platform.WebsocketService
{
    class ConsoleRunner
    {
        internal void Run()
        {
            AssemblyResolver.Initialize();
            AllocConsole();

            var serviceSetup = new ServicesSetup();
            serviceSetup.Setup();

            var brain = new Brain(serviceSetup.GetServiceProvider());
            brain.Start();

            Console.WriteLine("Started");
            Console.ReadLine();

            brain.Stop();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
