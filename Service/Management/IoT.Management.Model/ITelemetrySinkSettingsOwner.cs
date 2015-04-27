namespace Thriot.Management.Model
{
    public interface ITelemetrySinkSettingsOwner
    {
        TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
    }
}
