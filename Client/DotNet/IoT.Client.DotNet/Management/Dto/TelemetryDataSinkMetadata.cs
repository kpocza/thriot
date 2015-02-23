using System.Collections.Generic;

namespace IoT.Client.DotNet.Management
{
    public class TelemetryDataSinkMetadata
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> ParametersToInput { get; set; }    
    }
}