using System;
using System.Linq;
using Thriot.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thriot.Framework.Exceptions;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry
{
    public class TelemetryDataService
    {
        private readonly ITelemetryDataSinkResolver _telemetryDataSinkResolver;

        private const int IncomingMessageSizeLimit = 1024;

        public TelemetryDataService(ITelemetryDataSinkResolver telemetryDataSinkResolver)
        {
            _telemetryDataSinkResolver = telemetryDataSinkResolver;
        }

        public void RecordTelemetryData(string deviceId, JToken payload)
        {
            if (payload == null)
                throw new ArgumentNullException();

            if (deviceId == null)
                throw new ForbiddenException();

            payload.EnsureRecognizableFormat();

            var nativePayload = payload.ToString(Formatting.None);

            if (nativePayload.Length > IncomingMessageSizeLimit)
                throw new ArgumentException("Too long", "payload");

            var message = new TelemetryData(deviceId, nativePayload, DateTime.UtcNow);

            var telemetryDataSinks = _telemetryDataSinkResolver.ResolveIncoming(deviceId);

            if (!telemetryDataSinks.Any())
                throw new ArgumentException("No incoming telemetry data sinks registered");

            foreach (var messageOp in telemetryDataSinks)
            {
                messageOp.Record(message);
            }
        }
    }
}
