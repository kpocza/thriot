using System.Collections.Generic;

namespace Thriot.Platform.Services.Client
{
    public class TelemetryDataSinkParametersDtoClient
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; } 
    }
}