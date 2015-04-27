namespace Thriot.Management.Dto
{
    public class CompanyDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public TelemetryDataSinkSettingsDto TelemetryDataSinkSettings { get; set; }
    }
}