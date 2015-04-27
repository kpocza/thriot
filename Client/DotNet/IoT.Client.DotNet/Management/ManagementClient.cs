namespace Thriot.Client.DotNet.Management
{
    /// <summary>
    /// This is an umbrella class above all other classes responsible for doing management operations in case of 
    /// users, companies, services, networks, devices and telemetry data sinks.
    /// </summary>
    public class ManagementClient
    {
        /// <summary>
        /// Creates a new instance of the <exception cref="ManagementClient"></exception> class.
        /// </summary>
        /// <param name="baseUrl">The root Url for all management functions with It includes the version number.
        /// e.g. http://FQDN/URL/v1
        /// </param>
        public ManagementClient(string baseUrl) : this(baseUrl, new RestConnection())
        {
        }

        private ManagementClient(string baseUrl, IRestConnection restConnection)
        {
            restConnection.Setup(baseUrl, null);

            User = new UserManagementClient(baseUrl, restConnection);
            Company = new CompanyManagementClient(restConnection);
            Service = new ServiceManagementClient(restConnection);
            Network = new NetworkManagementClient(restConnection);
            Device = new DeviceManagementClient(restConnection);
            TelemetryDataSinksMetadata = new TelemetryDataSinksMetadataClient(restConnection);
        }

        /// <summary>
        /// User management functions
        /// </summary>
        public UserManagementClient User { get; private set; }

        /// <summary>
        /// Company management functions
        /// </summary>
        public CompanyManagementClient Company { get; private set; }
        
        /// <summary>
        /// Service management functions
        /// </summary>
        public ServiceManagementClient Service { get; private set; }
        
        /// <summary>
        /// Network management functions
        /// </summary>
        public NetworkManagementClient Network { get; private set; }
        
        /// <summary>
        /// Device management functions
        /// </summary>
        public DeviceManagementClient Device { get; private set; }
        
        /// <summary>
        /// Telemetry data sink metadata query functions
        /// </summary>
        public TelemetryDataSinksMetadataClient TelemetryDataSinksMetadata { get; private set; }
    }
}
