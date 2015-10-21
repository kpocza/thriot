using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers
{
    public interface ITelemetryEnvironment
    {
        string ConnectionStringParamName { get; }

        string ConnectionStringNameName { get; }

        string ConnectionString { get; }

        bool SupportsDuplicateCheck { get; }

        ITelemetryDataSinkCurrent DataSinkCurrent { get; }

        ITelemetryDataSinkTimeSeries DataSinkTimeSeries { get; }

        IDictionary<string, string> AdditionalSettings { get; }

    }
}
