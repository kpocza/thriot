namespace IoT.Client.DotNet.Management
{
    public class Service
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CompanyId { get; set; }

        public string ApiKey { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
    }
}