using System;
using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Objects.Model;
using IoT.Platform.Services.Telemetry.Metadata;
using IoT.Plugins.Core;

namespace IoT.Platform.Services.Telemetry
{
    public class TelemetryDataSinkPreparator
    {
        private readonly ITelemetryDataSinkMetadataRegistry _telemetryDataSinkMetadataRegistry;

        public TelemetryDataSinkPreparator(ITelemetryDataSinkMetadataRegistry telemetryDataSinkMetadataRegistry)
        {
            _telemetryDataSinkMetadataRegistry = telemetryDataSinkMetadataRegistry;
        }

        public void PrepareAndValidateIncoming(IEnumerable<TelemetryDataSinkParameters> telemetryDataSinkParameters)
        {
            foreach (var incoming in telemetryDataSinkParameters)
            {
                var telemetryDataSinkMetadata = _telemetryDataSinkMetadataRegistry.Incoming.SingleOrDefault(i => String.Equals(i.Name, incoming.SinkName, StringComparison.InvariantCultureIgnoreCase));

                if (telemetryDataSinkMetadata != null)
                {
                    if (!incoming.Parameters.Keys.All(telemetryDataSinkMetadata.ParametersToInput.Contains))
                        throw new ArgumentException("telemetryDataSinkParameters");

                    var op = (ITelemetryDataSink)SingleContainer.Instance.Resolve(telemetryDataSinkMetadata.Type);
                    var allParameters = telemetryDataSinkMetadata.ParametersPresets.Union(incoming.Parameters).ToDictionary(d => d.Key, d => d.Value);
                    op.Setup(allParameters);

                    op.Initialize();
                }
                else
                {
                    throw new ArgumentException("telemetryDataSinkParameters");
                }
            }
        }
    }
}
