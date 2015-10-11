using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class AzureTelemetryEnvironment : ITelemetryEnvironment
    {
        public string TelemetryConnectionString => "UseDevelopmentStorage=true";

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent => InstanceCreator.Create<ITelemetryDataSinkCurrent>("Thriot.Plugins.Azure.TelemetryDataSinkCurrent, Thriot.Plugins.Azure");

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries => InstanceCreator.Create<ITelemetryDataSinkTimeSeries>("Thriot.Plugins.Azure.TelemetryDataSinkTimeSeries, Thriot.Plugins.Azure");
    }
}
