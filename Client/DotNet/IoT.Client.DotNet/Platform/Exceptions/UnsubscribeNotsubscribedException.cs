using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// Trying to unsubscribe the device while it wasn't subscribed previously
    /// Used in case of persistent connection client.
    /// </summary>
    public class UnsubscribeNotsubscribedException : Exception
    {
    }
}
