using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// The device is already connected to the service
    /// Used in case of persistent connection client.
    /// </summary>
    public class ConnectedAlreadyException : Exception
    {
    }
}
