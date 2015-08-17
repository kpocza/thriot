using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Recording
{
    public abstract class TelemetryDataServiceBase : ITelemetryDataService
    {
        private const int IncomingMessageSizeLimit = 1024;
        protected readonly ITelemetryDataSinkResolver _telemetryDataSinkResolver;

        protected TelemetryDataServiceBase(ITelemetryDataSinkResolver telemetryDataSinkResolver)
        {
            _telemetryDataSinkResolver = telemetryDataSinkResolver;
        }

        public void RecordTelemetryData(string deviceId, JToken payload)
        {
            if (deviceId == null)
                throw new ForbiddenException();

            if (payload == null)
                throw new ArgumentNullException();

            payload.EnsureRecognizableFormat();

            var nativePayload = payload.ToString(Formatting.None);

            if (nativePayload.Length > IncomingMessageSizeLimit)
                throw new ArgumentException($"Too long for {deviceId}", nameof(payload));

            var telemetryData = new TelemetryData(deviceId, nativePayload, DateTime.UtcNow);

            var telemetryDataSinks = _telemetryDataSinkResolver.ResolveIncoming(deviceId);

            if (!telemetryDataSinks.Any())
                throw new ArgumentException($"No incoming telemetry data sinks registered for {deviceId}");

            RecordTelemetryDataWorker(telemetryDataSinks, telemetryData);
        }

        protected abstract void RecordTelemetryDataWorker(IEnumerable<ITelemetryDataSink> telemetryDataSinks, TelemetryData telemetryData);
    }
}