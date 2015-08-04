using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework.DataAccess;
using Thriot.Objects.Model;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry
{
    public class TelemetryDataSinkPreparator
    {
        private readonly ITelemetryDataSinkMetadataRegistry _telemetryDataSinkMetadataRegistry;
        private readonly IDynamicConnectionStringResolver _dynamicConnectionStringResolver;

        public TelemetryDataSinkPreparator(ITelemetryDataSinkMetadataRegistry telemetryDataSinkMetadataRegistry, IDynamicConnectionStringResolver dynamicConnectionStringResolver)
        {
            _telemetryDataSinkMetadataRegistry = telemetryDataSinkMetadataRegistry;
            _dynamicConnectionStringResolver = dynamicConnectionStringResolver;
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

                    var op = (ITelemetryDataSink)Activator.CreateInstance(telemetryDataSinkMetadata.Type);
                    var allParameters = telemetryDataSinkMetadata.ParametersPresets.Union(incoming.Parameters).ToDictionary(d => d.Key, d => d.Value);
                    op.Setup(_dynamicConnectionStringResolver, allParameters);

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
