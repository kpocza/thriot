using System;
using System.IO;
using System.Reflection;

namespace Thriot.Framework
{
    public static class AssemblyResolver
    {
        private static readonly object _lock = new object();

        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (_lock)
            {
                var pluginsDirectory = GetPluginsDirectory();
                var searchParent = Path.Combine(pluginsDirectory, "bin");

                foreach (var childDir in new DirectoryInfo(searchParent).GetDirectories())
                {
                    var childDirectoryPath = childDir.FullName;

                    var filePath = Path.Combine(childDirectoryPath, args.Name + ".dll");

                    if(File.Exists(filePath))
                        return Assembly.LoadFile(filePath);
                }

                return null;
            }
        }

        private static string GetPluginsDirectory()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            do
            {
                var pluginsDirectory = Path.Combine(currentDirectory, "plugins");
                if (Directory.Exists(pluginsDirectory))
                    return pluginsDirectory;

                var parentDirectory = Directory.GetParent(currentDirectory);
                if (parentDirectory == null)
                    return null;

                currentDirectory = parentDirectory.FullName;
            } while (true);
        }
    }
}
