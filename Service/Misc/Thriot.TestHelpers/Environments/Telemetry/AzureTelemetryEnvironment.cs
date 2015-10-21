using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class AzureTelemetryEnvironment : ITelemetryEnvironment
    {
        public string ConnectionStringParamName => "ConnectionString";

        public string ConnectionStringNameName => "ConnectionName";

        public string ConnectionString => "UseDevelopmentStorage=true";

        public bool SupportsDuplicateCheck => true;

        public ITelemetryDataSinkCurrent DataSinkCurrent => InstanceCreator.Create<ITelemetryDataSinkCurrent>("Thriot.Plugins.Azure.TelemetryDataSinkCurrent, Thriot.Plugins.Azure");

        public ITelemetryDataSinkTimeSeries DataSinkTimeSeries => InstanceCreator.Create<ITelemetryDataSinkTimeSeries>("Thriot.Plugins.Azure.TelemetryDataSinkTimeSeries, Thriot.Plugins.Azure");

        public IDictionary<string, string> AdditionalSettings => null;
    }
}
