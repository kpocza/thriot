using System.Collections.Generic;

namespace IoT.Client.DotNet.Management
{
    /// <summary>
    /// Class to encapsulate the list of telemetry data sinks with required metadata.
    /// </summary>
    public class TelemetryDataSinksMetadata
    {
        /// <summary>
        /// List of telemetry data sinks with required metadata.
        /// </summary>
        public List<TelemetryDataSinkMetadata> Incoming { get; set; }
    }
}