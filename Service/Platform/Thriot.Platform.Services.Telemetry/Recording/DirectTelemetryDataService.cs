using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public class DirectTelemetryDataService : TelemetryDataServiceBase
    {
        public DirectTelemetryDataService(ITelemetryDataSinkResolver telemetryDataSinkResolver) : base(telemetryDataSinkResolver)
        {
        }

        protected override void RecordTelemetryDataWorker(IEnumerable<ITelemetryDataSink> telemetryDataSinks, TelemetryData telemetryData)
        {
            foreach (var telemetryDataSink in telemetryDataSinks)
            {
                telemetryDataSink.Record(telemetryData);
            }
        }
    }
}
