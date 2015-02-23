using System.Collections.Generic;

namespace IoT.ServiceClient.TelemetrySetup
{
    public class TelemetryDataSinkParametersRemoteDto
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; } 
    }
}