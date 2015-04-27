using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// Subscription is required to access the operation
    /// Used in case of persistent connection client.
    /// </summary>
    public class SubscriptionRequiredException : Exception
    {
    }
}
