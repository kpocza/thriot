namespace IoT.Management.Model
{
    public interface ITelemetrySinkSettingsOwner
    {
        TelemetryDataSinkSettings TelemetryDataSinkSettings { get; set; }
    }
}
