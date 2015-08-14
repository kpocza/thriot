using Newtonsoft.Json.Linq;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public interface ITelemetryDataService
    {
        void RecordTelemetryData(string deviceId, JToken payload);
    }
}