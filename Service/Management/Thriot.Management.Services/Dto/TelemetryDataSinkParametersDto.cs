using System.Collections.Generic;

namespace Thriot.Management.Services.Dto
{
    public class TelemetryDataSinkParametersDto
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; } 
    }
}