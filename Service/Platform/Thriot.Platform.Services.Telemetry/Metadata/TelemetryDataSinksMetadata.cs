using System.Collections.Generic;

namespace Thriot.Platform.Services.Telemetry.Metadata
{
    public class TelemetryDataSinksMetadata
    {
        public TelemetryDataSinksMetadata(IEnumerable<TelemetryDataSinkMetadata> incoming)
        {
            Incoming = incoming;
        }

        public IEnumerable<TelemetryDataSinkMetadata> Incoming { get; private set; }
    }
}