using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;

namespace IoT.Framework.Logging
{
    public static class LoggerFactory
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ILogger GetCurrentClassLogger()
        {
            return new NLogLogger(LogManager.GetLogger(GetClassFullNameImpl()));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetClassFullNameImpl()
        {
            string className;
            Type declaringType;
            int framesToSkip = 2;

            do
            {
                var frame = new StackFrame(framesToSkip, false);
                var method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    className = method.Name;
                    break;
                }

                framesToSkip++;
                className = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return className;
        }
    }
}
