using Thriot.Plugins.Core;

namespace Thriot.TestHelpers
{
    public interface ITelemetryEnvironment
    {
        string TelemetryConnectionString { get; }

        ITelemetryDataSinkCurrent TelemetryDataSinkCurrent { get; }

        ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries { get; }
    }
}
