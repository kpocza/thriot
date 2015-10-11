using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class SqlTelemetryEnvironment : ITelemetryEnvironment
    {
        public string TelemetryConnectionString => @"Server=.\SQLEXPRESS;Database=ThriotTelemetry;Trusted_Connection=True;";

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent => InstanceCreator.Create<ITelemetryDataSinkCurrent>("Thriot.Plugins.Sql.TelemetryDataSinkCurrent, Thriot.Plugins.Sql");

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries => InstanceCreator.Create<ITelemetryDataSinkTimeSeries>("Thriot.Plugins.Sql.TelemetryDataSinkTimeSeries, Thriot.Plugins.Sql");
    }
}
