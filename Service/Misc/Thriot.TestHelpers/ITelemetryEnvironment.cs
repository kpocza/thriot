using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers
{
    public interface ITelemetryEnvironment
    {
        string ConnectionStringParamName { get; }

        string ConnectionString { get; }

        ITelemetryDataSinkCurrent DataSinkCurrent { get; }

        ITelemetryDataSinkTimeSeries DataSinkTimeSeries { get; }

        IDictionary<string, string> AdditionalSettings { get; }

    }
}
