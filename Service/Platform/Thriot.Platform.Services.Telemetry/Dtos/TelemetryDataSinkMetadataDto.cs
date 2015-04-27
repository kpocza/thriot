using System.Collections.Generic;

namespace Thriot.Platform.Services.Telemetry.Dtos
{
    public class TelemetryDataSinkMetadataDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string TypeName { get; set; }

        public List<string> ParametersToInput { get; set; }

        public Dictionary<string, string> ParametersPresets { get; set; }
    }
}