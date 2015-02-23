using System.Collections.Generic;

namespace IoT.Platform.Services.Telemetry.Metadata
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