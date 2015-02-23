namespace IoT.Client.DotNet.Management
{
    public class ManagementClient
    {
        public ManagementClient(string baseUrl, IRestConnection restConnection)
        {
            restConnection.Setup(baseUrl, null);

            User = new UserManagementClient(baseUrl, restConnection);
            Company = new CompanyManagementClient(restConnection);
            Service = new ServiceManagementClient(restConnection);
            Network = new NetworkManagementClient(restConnection);
            Device = new DeviceManagementClient(restConnection);
            TelemetryDataSinksMetadata = new TelemetryDataSinksMetadataClient(restConnection);
        }

        public UserManagementClient User { get; private set; }

        public CompanyManagementClient Company { get; private set; }
        
        public ServiceManagementClient Service { get; private set; }
        
        public NetworkManagementClient Network { get; private set; }
        
        public DeviceManagementClient Device { get; private set; }
        
        public TelemetryDataSinksMetadataClient TelemetryDataSinksMetadata { get; private set; }
    }
}
