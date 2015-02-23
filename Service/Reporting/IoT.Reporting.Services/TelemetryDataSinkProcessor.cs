using System;
using System.Collections.Generic;
using System.Linq;
using IoT.Framework;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Plugins.Core;
using IoT.Reporting.Dto;
using IoT.ServiceClient.TelemetrySetup;

namespace IoT.Reporting.Services
{
    public class TelemetryDataSinkProcessor : ITelemetryDataSinkProcessor
    {
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;
        private readonly ICompanyOperations _companyOperations;
        private readonly ITelemetryDataSinkSetupService _telemetryDataSinkSetupService;

        public TelemetryDataSinkProcessor(ITelemetryDataSinkSetupService telemetryDataSinkSetupService, INetworkOperations networkOperations, IServiceOperations serviceOperations, ICompanyOperations companyOperations)
        {
            _telemetryDataSinkSetupService = telemetryDataSinkSetupService;
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
            _companyOperations = companyOperations;
        }

        public IEnumerable<SinkInfo> GetSinksForNetwork(string networkId)
        {
            var list = new List<SinkInfo>();
            var allSinks = GetConfiguredSinks(networkId);

            var telemetryDataSinksMetadata = _telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();

            foreach (var sink in allSinks)
            {
                var telemetryDataSinkMetadata = telemetryDataSinksMetadata.Incoming.SingleOrDefault(sinkMeta => String.Equals(sinkMeta.Name, sink.SinkName, StringComparison.InvariantCultureIgnoreCase));
                if (telemetryDataSinkMetadata != null)
                {
                    if (typeof(ITelemetryDataSinkCurrent).IsAssignableFrom(Type.GetType(telemetryDataSinkMetadata.TypeName)))
                    {
                        list.Add(new SinkInfo { SinkName = sink.SinkName, SinkType = SinkType.CurrentData });
                    }
                    else if (typeof(ITelemetryDataSinkTimeSeries).IsAssignableFrom(Type.GetType(telemetryDataSinkMetadata.TypeName)))
                    {
                        list.Add(new SinkInfo { SinkName = sink.SinkName, SinkType = SinkType.TimeSeries });
                    }
                }
            }

            return list;
        }

        public ITelemetryDataSink WorkerTelemetryDataSink(string sinkName, string networkId)
        {
            var configuredSinks = GetConfiguredSinks(networkId);

            var telemetryDataSinksMetadata = _telemetryDataSinkSetupService.GetTelemetryDataSinksMetadata();

            var workerSink = configuredSinks.SingleOrDefault(sink => String.Equals(sink.SinkName, sinkName, StringComparison.InvariantCultureIgnoreCase));
            if (workerSink == null)
                return null;

            var telemetryDataSinkMetadata = telemetryDataSinksMetadata.Incoming.Single(sink => String.Equals(sink.Name, sinkName, StringComparison.InvariantCultureIgnoreCase));

            var telemetryDataSink = (ITelemetryDataSink)SingleContainer.Instance.Resolve(Type.GetType(telemetryDataSinkMetadata.TypeName));
            var allParameters = telemetryDataSinkMetadata.ParametersPresets.Union(workerSink.Parameters).ToDictionary(d => d.Key, d => d.Value);
            telemetryDataSink.Setup(allParameters);

            return telemetryDataSink;
        }

        private IEnumerable<TelemetryDataSinkParameters> GetConfiguredSinks(string networkId)
        {
            var network = _networkOperations.Get(networkId);
            var serviceId = network.ServiceId;
            var companyId = network.CompanyId;

            while (network != null)
            {
                if (network.TelemetryDataSinkSettings.Incoming != null && network.TelemetryDataSinkSettings.Incoming.Any())
                {
                    return network.TelemetryDataSinkSettings.Incoming;
                }

                network = network.ParentNetworkId != null ? _networkOperations.Get(network.ParentNetworkId) : null;
            }

            var service = _serviceOperations.Get(serviceId);
            if (service.TelemetryDataSinkSettings.Incoming != null && service.TelemetryDataSinkSettings.Incoming.Any())
            {
                return service.TelemetryDataSinkSettings.Incoming;
            }

            var company = _companyOperations.Get(companyId);
            if (company.TelemetryDataSinkSettings.Incoming != null && company.TelemetryDataSinkSettings.Incoming.Any())
            {
                return company.TelemetryDataSinkSettings.Incoming;
            }

            return new List<TelemetryDataSinkParameters>();
        }
    }
}