using System.Collections.Generic;

namespace Thriot.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinksMetadataDto
    {
        public List<TelemetryDataSinkMetadataDto> Incoming { get; set; }
    }
}