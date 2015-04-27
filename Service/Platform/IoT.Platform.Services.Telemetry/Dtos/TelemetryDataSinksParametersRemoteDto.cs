using System.Collections.Generic;

namespace Thriot.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinksParametersRemoteDto
    {
        public List<TelemetryDataSinkParametersRemoteDto> Incoming { get; set; } 
    }
}