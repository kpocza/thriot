using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class PgSqlTelemetryEnvironment : ITelemetryEnvironment
    {
        public string TelemetryConnectionString => "Server=127.0.0.1;Port=5432;Database=ThriotTelemetry;User Id=thriottelemetry;Password=thriottelemetry;";

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent => InstanceCreator.Create<ITelemetryDataSinkCurrent>("Thriot.Plugins.PgSql.TelemetryDataSinkCurrent, Thriot.Plugins.PgSql");

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries => InstanceCreator.Create<ITelemetryDataSinkTimeSeries>("Thriot.Plugins.PgSql.TelemetryDataSinkTimeSeries, Thriot.Plugins.PgSql");
    }
}
