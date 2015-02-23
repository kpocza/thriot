using System.Collections.Generic;

namespace IoT.Management.Dto
{
    public class TelemetryDataSinkMetadataDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> ParametersToInput { get; set; }
    }
}