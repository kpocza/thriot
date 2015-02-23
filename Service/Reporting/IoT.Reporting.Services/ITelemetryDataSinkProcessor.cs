using System.Collections.Generic;
using IoT.Plugins.Core;

namespace IoT.Reporting.Services
{
    public interface ITelemetryDataSinkProcessor
    {
        IEnumerable<SinkInfo> GetSinksForNetwork(string networkId);

        ITelemetryDataSink WorkerTelemetryDataSink(string sinkName, string networkId);
    }
}
