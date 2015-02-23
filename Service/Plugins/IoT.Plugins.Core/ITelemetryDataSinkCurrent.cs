using System.Collections.Generic;

namespace IoT.Plugins.Core
{
    public interface ITelemetryDataSinkCurrent : ITelemetryDataSink
    {
        IEnumerable<TelemetryData> GetCurrentData(IEnumerable<string> deviceIds);
    }
}