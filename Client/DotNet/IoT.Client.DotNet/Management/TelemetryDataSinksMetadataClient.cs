namespace IoT.Client.DotNet.Management
{
    /// <summary>
    /// This class is responsible to query the applicable telemetry data sinks. The list of telemetry data sinks is needed to
    /// know which sinks are available with which parameters. Telemetry data sinks are used configure where the telemetry data sent by the 
    /// devices will be recorded.
    /// </summary>
    public class TelemetryDataSinksMetadataClient : SpecificManagementClient
    {
        internal TelemetryDataSinksMetadataClient(IRestConnection restConnection)
            : base(restConnection)
        {
        }

        /// <summary>
        /// Query the telemetry data sinks list with all required metadata
        /// 
        /// Send GET request to the APIROOT/telemetryMetadata Url
        /// </summary>
        /// <returns>List of applicable telemetry data sinks with metadata</returns>
        public TelemetryDataSinksMetadata Get()
        {
            var response = RestConnection.Get("telemetryMetadata");

            return JsonSerializer.Deserialize<TelemetryDataSinksMetadata>(response);
        }
    }
}
