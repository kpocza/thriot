using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// Error on commiting message
    /// Used in case of persistent connection client.
    /// </summary>
    public class CommitInvalidException : Exception
    {
        /// <summary>
        /// Commit error response
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Creates a commit error exception
        /// </summary>
        /// <param name="response">Response error message from service</param>
        public CommitInvalidException(string response)
        {
            Response = response;
        }
    }
}
