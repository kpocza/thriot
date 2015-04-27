using System.Collections.Generic;

namespace Thriot.Platform.Services.Telemetry.Metadata
{
    public interface ITelemetryDataSinkMetadataRegistry
    {
        IEnumerable<TelemetryDataSinkMetadata> Incoming { get; }
    }
}