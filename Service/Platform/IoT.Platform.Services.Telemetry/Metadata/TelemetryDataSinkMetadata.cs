using System;
using System.Collections.Generic;

namespace IoT.Platform.Services.Telemetry.Metadata
{
    public class TelemetryDataSinkMetadata
    {
        public TelemetryDataSinkMetadata(string name, string description, Type type, IEnumerable<string> parametersToInput, IDictionary<string, string> parametersPresets)
        {
            Name = name;
            Description = description;
            Type = type;
            ParametersToInput = parametersToInput;
            ParametersPresets = parametersPresets;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Type Type { get; private set; }

        public IEnumerable<string> ParametersToInput { get; private set; }

        public IDictionary<string, string> ParametersPresets { get; private set; }
    }
}
