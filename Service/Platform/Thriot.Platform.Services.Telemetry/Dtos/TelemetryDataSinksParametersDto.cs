using System.Collections.Generic;

namespace Thriot.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinksParametersDto
    {
        public List<TelemetryDataSinkParametersDto> Incoming { get; set; } 
    }
}