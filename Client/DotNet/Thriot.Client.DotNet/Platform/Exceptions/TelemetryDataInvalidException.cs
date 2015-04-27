using System;

namespace Thriot.Client.DotNet.Platform.Exceptions
{
    /// <summary>
    /// Invalid telemetry data
    /// Used in case of persistent connection client.
    /// </summary>
    public class TelemetryDataInvalidException : Exception
    {
        /// <summary>
        /// Error message from the service
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Create a new exception instance
        /// </summary>
        /// <param name="response">Error message</param>
        public TelemetryDataInvalidException(string response)
        {
            Response = response;
        }
    }
}
