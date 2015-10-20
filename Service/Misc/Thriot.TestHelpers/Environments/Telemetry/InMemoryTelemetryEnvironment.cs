using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class InMemoryTelemetryEnvironment : ITelemetryEnvironment
    {
        public string ConnectionStringParamName => "ConnectionString";

        public string ConnectionString => null;

        public ITelemetryDataSinkCurrent DataSinkCurrent => null;

        public ITelemetryDataSinkTimeSeries DataSinkTimeSeries => null;

        public IDictionary<string, string> AdditionalSettings => null;
    }
}
