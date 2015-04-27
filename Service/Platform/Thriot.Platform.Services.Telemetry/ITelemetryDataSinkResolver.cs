using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry
{
    public interface ITelemetryDataSinkResolver
    {
        IEnumerable<ITelemetryDataSink> ResolveIncoming(string deviceId);
    }
}
