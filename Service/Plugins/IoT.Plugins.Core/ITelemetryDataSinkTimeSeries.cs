using System;
using System.Collections.Generic;

namespace Thriot.Plugins.Core
{
    public interface ITelemetryDataSinkTimeSeries : ITelemetryDataSink
    {
        IEnumerable<TelemetryData> GetTimeSeries(IEnumerable<string> deviceIds, DateTime date);
    }
}