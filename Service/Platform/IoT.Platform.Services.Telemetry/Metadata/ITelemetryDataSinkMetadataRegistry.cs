using System.Collections.Generic;

namespace IoT.Platform.Services.Telemetry.Metadata
{
    public interface ITelemetryDataSinkMetadataRegistry
    {
        IEnumerable<TelemetryDataSinkMetadata> Incoming { get; }
    }
}