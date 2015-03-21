namespace IoT.Client.DotNet.Reporting
{
    public class ReportingClient
    {
        public ReportingClient(string baseUrl) : this(baseUrl, new RestConnection())
        {
        }

        private ReportingClient(string baseUrl, IRestConnection restConnection)
        {
            restConnection.Setup(baseUrl, null);

            Device = new DeviceClient(baseUrl, restConnection);
            Network = new NetworkClient(baseUrl, restConnection);
        }

        public DeviceClient Device { get; private set; }

        public NetworkClient Network { get; private set; }
    }
}
