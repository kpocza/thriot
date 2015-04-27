using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// There was an error while subscribing
    /// Used in case of persistent connection client.
    /// </summary>
    public class SubscribeInvalidException : Exception
    {
        /// <summary>
        /// Error response from the service
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Create exception instance
        /// </summary>
        /// <param name="response">Error response</param>
        public SubscribeInvalidException(string response)
        {
            Response = response;
        }
    }
}
