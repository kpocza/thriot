using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// Login required to access the operations
    /// Used in case of persistent connection client.
    /// </summary>
    public class LoginRequiredException : Exception
    {
    }
}
