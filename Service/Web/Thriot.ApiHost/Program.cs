using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;

namespace Thriot.ApiHost
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "/run")
            {
                var baseAddress = args[1];

                Console.WriteLine("Base address: " + args[1]);

                using (WebApp.Start(baseAddress))
                {
                    Console.WriteLine("Running...");
                    Console.ReadLine();
                }
            }
            else
            {
                PrepareSandboxes();
            }
        }

        private static void PrepareSandboxes()
        {
            var rootPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            var configPath = Path.Combine(rootPath, "host.config.json");

            var host = JsonConvert.DeserializeObject<Host>(File.ReadAllText(configPath));
            var sandboxPath = Path.Combine(rootPath, "sandbox");

            if (!Directory.Exists(sandboxPath))
            {
                Directory.CreateDirectory(sandboxPath);
            }

            var applicationPath = Path.Combine(sandboxPath, host.Name);

            if (Directory.Exists(applicationPath))
            {
                Directory.Delete(applicationPath, true);
            }

            Directory.CreateDirectory(applicationPath);

            var sourceApplicationPath = Path.Combine(rootPath, host.Application);
            var sourceAppConfigPath = Path.Combine(rootPath, host.Config);
            var appConfigPath = Path.Combine(applicationPath, "Thriot.ApiHost.exe.config");

            DirectoryCopy(sourceApplicationPath, applicationPath, true);

            PrepareBin(applicationPath);

            File.Copy(sourceAppConfigPath, appConfigPath);

            foreach (var file in new DirectoryInfo(rootPath).GetFiles("*.*"))
            {
                var ext = Path.GetExtension(file.FullName);

                if (new[] {".exe", ".dll", ".pdb"}.Contains(ext))
                {
                    File.Copy(file.FullName, Path.Combine(applicationPath, file.Name), true);
                }
            }

            var psi = new ProcessStartInfo
            {
                Arguments = string.Format("/run \"{0}\"", host.BaseAddress),
                FileName = Path.Combine(applicationPath, "Thriot.ApiHost.exe")
            };
            Process.Start(psi);
        }

        private static void PrepareBin(string applicationPath)
        {
            var binPath = Path.Combine(applicationPath, "bin");
            foreach (var file in new DirectoryInfo(binPath).GetFiles())
            {
                File.Move(file.FullName, Path.Combine(applicationPath, file.Name));
            }
            foreach (var dir in new DirectoryInfo(binPath).GetDirectories())
            {
                Directory.Move(dir.FullName, Path.Combine(applicationPath, dir.Name));
            }

            Directory.Delete(binPath);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            var files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
