namespace Thriot.Management.Dto
{
    public class ServiceDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CompanyId { get; set; }

        public string ApiKey { get; set; }

        public TelemetryDataSinkSettingsDto TelemetryDataSinkSettings { get; set; }
    }
}