using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public class QueueingTelemetryDataService : TelemetryDataServiceBase
    {
        private readonly IQueueSendAdapter _queueSendAdapter;

        public QueueingTelemetryDataService(ITelemetryDataSinkResolver telemetryDataSinkResolver, IQueueSendAdapter queueSendAdapter) : base(telemetryDataSinkResolver)
        {
            _queueSendAdapter = queueSendAdapter;
        }

        protected override void RecordTelemetryDataWorker(IEnumerable<ITelemetryDataSink> telemetryDataSinks, TelemetryData telemetryData)
        {
            _queueSendAdapter.Send(telemetryData);
        }
    }
}
