using System.Collections.Generic;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Telemetry data sink metadata
    /// </summary>
    public class TelemetryDataSinkMetadata
    {
        /// <summary>
        /// Name of the telemetry data sink
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short description of the telemetry data sink
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of parameter names that need to be specified by the user when configuring an instance of the telemetry data sink
        /// Such parameters can be connection strings, urls, authentication information, tables, etc.
        /// </summary>
        public List<string> ParametersToInput { get; set; }    
    }
}