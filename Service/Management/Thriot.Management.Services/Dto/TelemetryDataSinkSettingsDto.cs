using System.Collections.Generic;

namespace Thriot.Management.Services.Dto
{
    public class TelemetryDataSinkSettingsDto
    {
        public List<TelemetryDataSinkParametersDto> Incoming { get; set; } 
    }
}