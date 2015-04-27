using System.Collections.Generic;

namespace Thriot.Plugins.Core
{
    public interface ITelemetryDataSinkCurrent : ITelemetryDataSink
    {
        IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds);
    }
}