using System;

namespace Thriot.Framework
{
    public class Environment
    {
        public static bool IsMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
