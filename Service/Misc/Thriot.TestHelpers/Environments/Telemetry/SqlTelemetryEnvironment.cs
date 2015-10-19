using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class SqlTelemetryEnvironment : ITelemetryEnvironment
    {
        public string ConnectionStringParamName => "ConnectionString";

        public string ConnectionString => @"Server=.\SQLEXPRESS;Database=ThriotTelemetry;Trusted_Connection=True;";

        public ITelemetryDataSinkCurrent DataSinkCurrent => InstanceCreator.Create<ITelemetryDataSinkCurrent>("Thriot.Plugins.Sql.TelemetryDataSinkCurrent, Thriot.Plugins.Sql");

        public ITelemetryDataSinkTimeSeries DataSinkTimeSeries => InstanceCreator.Create<ITelemetryDataSinkTimeSeries>("Thriot.Plugins.Sql.TelemetryDataSinkTimeSeries, Thriot.Plugins.Sql");

        public IDictionary<string, string> AdditionalSettings => null;
    }
}
