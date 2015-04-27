using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// Error while sending message to a device
    /// Used in case of persistent connection client.
    /// </summary>
    public class SendToInvalidException : Exception
    {
        /// <summary>
        /// Error response
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Create a new exception instance
        /// </summary>
        /// <param name="response">Error response</param>
        public SendToInvalidException(string response)
        {
            Response = response;
        }
    }
}
