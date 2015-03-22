using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// The device is already subscribed to pushed messaged
    /// Used in case of persistent connection client.
    /// </summary>
    public class SubscribedAlreadyException : Exception
    {
    }
}
