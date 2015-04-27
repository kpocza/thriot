namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// Entity with all the details a company publicly exposes
    /// </summary>
    public class Company
    {
        /// <summary>
        /// Unique identifier (generally 32 characters long)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the entity to be able to identify the entity by a human
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Telemetry data sink instances configured the way it's specified by telemetry data sinks' meta data
        /// </summary>
        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
    }
}