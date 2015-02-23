namespace IoT.Client.DotNet.Management
{
    public class Company
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
    }
}