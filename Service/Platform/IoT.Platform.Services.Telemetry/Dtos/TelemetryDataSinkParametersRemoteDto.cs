using System.Collections.Generic;

namespace IoT.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinkParametersRemoteDto
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; } 
    }
}