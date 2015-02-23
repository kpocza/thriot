using System.Collections.Generic;

namespace IoT.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinksParametersRemoteDto
    {
        public List<TelemetryDataSinkParametersRemoteDto> Incoming { get; set; } 
    }
}