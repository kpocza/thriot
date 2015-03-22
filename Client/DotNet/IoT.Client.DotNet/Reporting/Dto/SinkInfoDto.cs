namespace IoT.Client.DotNet.Reporting
{
    /// <summary>
    /// Telemetry data sink information for reporting
    /// </summary>
    public class SinkInfoDto
    {
        /// <summary>
        /// Name of the telemetry data sink
        /// </summary>
        public string SinkName { get; set; }

        /// <summary>
        /// Sink type. Current data or time series
        /// </summary>
        public SinkType SinkType { get; set; }
    }
}
