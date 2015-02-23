using System.Collections.Generic;

namespace IoT.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinksMetadataDto
    {
        public List<TelemetryDataSinkMetadataDto> Incoming { get; set; }
    }
}