using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// Error while unsubscribing the device from the pushed messages.
    /// Used in case of persistent connection client.
    /// </summary>
    public class UnsubscribeInvalidException : Exception
    {
        /// <summary>
        /// Response error message
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Create exception instance
        /// </summary>
        /// <param name="response">Error message</param>
        public UnsubscribeInvalidException(string response)
        {
            Response = response;
        }
    }
}
