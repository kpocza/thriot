namespace IoT.Client.DotNet.Reporting
{
    /// <summary>
    /// Umbrella class for device and network based reporting functions.
    /// The reporting functions are suitable for creating JSON and CSV output based on the recorded telemetry data of single or multiple devices.
    /// </summary>
    public class ReportingClient
    {
        /// <summary>
        /// Creates a new instance of reporting client
        /// </summary>
        /// <param name="baseUrl">Reporting API root</param>
        public ReportingClient(string baseUrl) : this(baseUrl, new RestConnection())
        {
        }

        private ReportingClient(string baseUrl, IRestConnection restConnection)
        {
            restConnection.Setup(baseUrl, null);

            Device = new DeviceClient(baseUrl, restConnection);
            Network = new NetworkClient(baseUrl, restConnection);
        }

        /// <summary>
        /// Device-specific reporting functions
        /// </summary>
        public DeviceClient Device { get; private set; }

        /// <summary>
        /// Network-specific reporting functions
        /// </summary>
        public NetworkClient Network { get; private set; }
    }
}
