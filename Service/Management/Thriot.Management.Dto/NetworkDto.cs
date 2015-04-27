namespace Thriot.Management.Dto
{
    public class NetworkDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentNetworkId { get; set; }

        public string ServiceId { get; set; }

        public string CompanyId { get; set; }

        public string NetworkKey { get; set; }

        public TelemetryDataSinkSettingsDto TelemetryDataSinkSettings { get; set; }
    }
}