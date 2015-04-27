using System.Collections.Generic;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Incoming telemetry data sink with parameters
    /// </summary>
    public class TelemetryDataSinkParameters
    {
        /// <summary>
        /// Name of the incoming telemetry data sink
        /// </summary>
        public string SinkName { get; set; }

        /// <summary>
        /// Dictionary of parameters configured for the telemetry data sink (key is the parameter name, value is the value)
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } 
    }
}