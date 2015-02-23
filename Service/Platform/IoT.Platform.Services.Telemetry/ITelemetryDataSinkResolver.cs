using System.Collections.Generic;
using IoT.Plugins.Core;

namespace IoT.Platform.Services.Telemetry
{
    public interface ITelemetryDataSinkResolver
    {
        IEnumerable<ITelemetryDataSink> ResolveIncoming(string deviceId);
    }
}
