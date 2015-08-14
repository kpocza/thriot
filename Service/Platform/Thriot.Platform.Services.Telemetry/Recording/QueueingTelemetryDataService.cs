using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public class QueueingTelemetryDataService : TelemetryDataServiceBase
    {
        private readonly IQueueAdapter _queueAdapter;

        public QueueingTelemetryDataService(ITelemetryDataSinkResolver telemetryDataSinkResolver, IQueueAdapter queueAdapter) : base(telemetryDataSinkResolver)
        {
            _queueAdapter = queueAdapter;
        }

        protected override void RecordTelemetryDataWorker(IEnumerable<ITelemetryDataSink> telemetryDataSinks, TelemetryData telemetryData)
        {
            _queueAdapter.Send(telemetryData);
        }
    }
}
