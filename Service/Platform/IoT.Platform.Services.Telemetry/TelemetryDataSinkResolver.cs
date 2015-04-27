using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Services.Telemetry.Metadata;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry
{
    public class TelemetryDataSinkResolver : ITelemetryDataSinkResolver
    {
        private readonly IDeviceOperations _deviceOperations;
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;
        private readonly ICompanyOperations _companyOperations;
        private readonly ITelemetryDataSinkMetadataRegistry _telemetryDataSinkMetadataRegistry;

        public TelemetryDataSinkResolver(IDeviceOperations deviceOperations, INetworkOperations networkOperations, IServiceOperations serviceOperations, ICompanyOperations companyOperations, ITelemetryDataSinkMetadataRegistry telemetryDataSinkMetadataRegistry)
        {
            _deviceOperations = deviceOperations;
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
            _companyOperations = companyOperations;
            _telemetryDataSinkMetadataRegistry = telemetryDataSinkMetadataRegistry;
        }

        public IEnumerable<ITelemetryDataSink> ResolveIncoming(string deviceId)
        {
            var device = _deviceOperations.Get(deviceId);

            var network = _networkOperations.Get(device.NetworkId);
            while (network != null)
            {
                if (network.TelemetryDataSinkSettings.Incoming != null && network.TelemetryDataSinkSettings.Incoming.Any())
                {
                    return ResolveIncomingInternal(network.TelemetryDataSinkSettings.Incoming);
                }

                network = network.ParentNetworkId != null ? _networkOperations.Get(network.ParentNetworkId) : null;
            }

            var service = _serviceOperations.Get(device.ServiceId);
            if (service.TelemetryDataSinkSettings.Incoming!=null && service.TelemetryDataSinkSettings.Incoming.Any())
            {
                return ResolveIncomingInternal(service.TelemetryDataSinkSettings.Incoming);
            }

            var company = _companyOperations.Get(device.CompanyId);
            if (company.TelemetryDataSinkSettings.Incoming!= null && company.TelemetryDataSinkSettings.Incoming.Any())
            {
                return ResolveIncomingInternal(company.TelemetryDataSinkSettings.Incoming);
            }

            return new List<ITelemetryDataSink>();
        }

        private IEnumerable<ITelemetryDataSink> ResolveIncomingInternal(IEnumerable<TelemetryDataSinkParameters> incomings)
        {
            var ops = new List<ITelemetryDataSink>();

            foreach (var incoming in incomings)
            {
                var telemetryDataSinkMetadata = _telemetryDataSinkMetadataRegistry.Incoming.SingleOrDefault(i => String.Equals(i.Name, incoming.SinkName, StringComparison.InvariantCultureIgnoreCase));

                if (telemetryDataSinkMetadata != null)
                {
                    var telemetryDataSink = (ITelemetryDataSink)SingleContainer.Instance.Resolve(telemetryDataSinkMetadata.Type);
                    var allParameters = telemetryDataSinkMetadata.ParametersPresets.Union(incoming.Parameters).ToDictionary(d => d.Key, d => d.Value);
                    telemetryDataSink.Setup(allParameters);

                    ops.Add(telemetryDataSink);
                }
            }

            return ops;
        }
    }
}
