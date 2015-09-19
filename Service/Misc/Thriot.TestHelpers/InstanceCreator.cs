using System;

namespace Thriot.TestHelpers
{
    internal static class InstanceCreator
    {
        internal static T Create<T>(string type)
        {
            return (T) Activator.CreateInstance(Type.GetType(type));
        }
    }
}
