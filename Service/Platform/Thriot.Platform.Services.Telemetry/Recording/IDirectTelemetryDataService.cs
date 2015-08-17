using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public interface IDirectTelemetryDataService : ITelemetryDataService
    {
        void RecordTelemetryData(TelemetryData telemetryData);
    }
}