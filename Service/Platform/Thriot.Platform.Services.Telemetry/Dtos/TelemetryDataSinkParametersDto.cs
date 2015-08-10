using System.Collections.Generic;

namespace Thriot.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinkParametersDto
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; } 
    }
}