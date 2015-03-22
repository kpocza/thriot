namespace IoT.Client.DotNet.Management
{
    /// <summary>
    /// Entity with all publically exposed properties by a Service
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Unique identifier of the service
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Human readable name of the service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Enclosing company
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Crypto-random Api key of the service. All devices can use this along with the device id for authentication that are under this service.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Telemetry data sink instances with parameters set up
        /// </summary>
        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
    }
}