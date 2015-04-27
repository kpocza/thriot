using System.Collections.Generic;

namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Class that encapuslated the list of configured telemetry data sinks for incoming telemetry data recording
    /// </summary>
    public class TelemetryDataSinkSettings
    {
        /// <summary>
        /// List of configured telemetry data sinks for incoming telemetry data recording
        /// </summary>
        public List<TelemetryDataSinkParameters> Incoming { get; set; } 
    }
}