using System.Collections.Generic;

namespace IoT.Objects.Model
{
    public class TelemetryDataSinkParameters
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
