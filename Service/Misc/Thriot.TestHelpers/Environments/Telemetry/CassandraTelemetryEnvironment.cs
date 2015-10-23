using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Telemetry
{
    public class CassandraTelemetryEnvironment : ITelemetryEnvironment
    {
        public string ConnectionStringParamName => "ContactPoints";

        public string ConnectionStringNameName => "ContactPointName";

        public string ConnectionString => "ubuntucas1";

        // to be exact it supports but with performance penalty
        public bool SupportsDuplicateCheck => false;

        public ITelemetryDataSinkCurrent DataSinkCurrent => InstanceCreator.Create<ITelemetryDataSinkCurrent>("Thriot.Plugins.Cassandra.TelemetryDataSinkCurrent, Thriot.Plugins.Cassandra");

        public ITelemetryDataSinkTimeSeries DataSinkTimeSeries => InstanceCreator.Create<ITelemetryDataSinkTimeSeries>("Thriot.Plugins.Cassandra.TelemetryDataSinkTimeSeries, Thriot.Plugins.Cassandra");

        public IDictionary<string, string> AdditionalSettings => new Dictionary<string, string> {{"Keyspace", "test"}};
    }
}
