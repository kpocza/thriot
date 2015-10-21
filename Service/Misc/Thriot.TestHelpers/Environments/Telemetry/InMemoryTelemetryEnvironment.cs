using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class InMemoryTelemetryEnvironment : ITelemetryEnvironment
    {
        public string ConnectionStringParamName => "ConnectionString";

        public string ConnectionStringNameName => "ConnectionName";

        public string ConnectionString => null;

        public bool SupportsDuplicateCheck => true;

        public ITelemetryDataSinkCurrent DataSinkCurrent => null;

        public ITelemetryDataSinkTimeSeries DataSinkTimeSeries => null;

        public IDictionary<string, string> AdditionalSettings => null;
    }
}
