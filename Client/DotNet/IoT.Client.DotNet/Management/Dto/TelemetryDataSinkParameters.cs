using System.Collections.Generic;

namespace IoT.Client.DotNet.Management
{
    public class TelemetryDataSinkParameters
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; } 
    }
}