using System;

namespace IoT.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// There was an error while login
    /// Used in case of persistent connection client.
    /// </summary>
    public class LoginInvalidException : Exception
    {
        /// <summary>
        /// Login error response message
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Create a new exception instance
        /// </summary>
        /// <param name="response">Response error message</param>
        public LoginInvalidException(string response)
        {
            Response = response;
        }
    }
}
