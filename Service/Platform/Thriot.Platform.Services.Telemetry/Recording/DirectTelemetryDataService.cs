using System;
using System.Linq;
using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public class DirectTelemetryDataService : TelemetryDataServiceBase, IDirectTelemetryDataService
    {
        public DirectTelemetryDataService(ITelemetryDataSinkResolver telemetryDataSinkResolver) : base(telemetryDataSinkResolver)
        {
        }

        public void RecordTelemetryData(TelemetryData telemetryData)
        {
            var deviceId = telemetryData.DeviceId;
            var telemetryDataSinks = _telemetryDataSinkResolver.ResolveIncoming(deviceId);

            if (!telemetryDataSinks.Any())
                throw new ArgumentException($"No incoming telemetry data sinks registered for {deviceId}");

            RecordTelemetryDataWorker(telemetryDataSinks, telemetryData);
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
