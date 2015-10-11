using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class InMemoryTelemetryEnvironment : ITelemetryEnvironment
    {
        public string TelemetryConnectionString => null;

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent => null;

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries => null;
    }
}
