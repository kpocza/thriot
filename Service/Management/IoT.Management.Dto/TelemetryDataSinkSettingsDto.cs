using System.Collections.Generic;

namespace IoT.Management.Dto
{
    public class TelemetryDataSinkSettingsDto
    {
        public List<TelemetryDataSinkParametersDto> Incoming { get; set; } 
    }
}