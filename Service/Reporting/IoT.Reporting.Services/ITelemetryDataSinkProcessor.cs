using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Reporting.Services
{
    public interface ITelemetryDataSinkProcessor
    {
        IEnumerable<SinkInfo> GetSinksForNetwork(string networkId);

        ITelemetryDataSink WorkerTelemetryDataSink(string sinkName, string networkId);
    }
}
