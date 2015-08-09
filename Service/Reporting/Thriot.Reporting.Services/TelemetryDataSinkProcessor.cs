using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Framework;
using Thriot.Framework.DataAccess;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Plugins.Core;
using Thriot.Reporting.Services.Dto;
using Thriot.ServiceClient.TelemetrySetup;

namespace Thriot.Reporting.Services
{
    public class TelemetryDataSinkProcessor : ITelemetryDataSinkProcessor
    {
        private readonly INetworkOperations _networkOperations;
        private readonly IServiceOperations _serviceOperations;
        private readonly ICompanyOperations _companyOperations;
        private readonly ITelemetryDataSinkSetupServiceClient _telemetryDataSinkSetupServiceClient;
        private readonly IDynamicConnectionStringResolver _dynamicConnectionStringResolver;

        public TelemetryDataSinkProcessor(ITelemetryDataSinkSetupServiceClient telemetryDataSinkSetupServiceClient, INetworkOperations networkOperations, IServiceOperations serviceOperations, ICompanyOperations companyOperations, IDynamicConnectionStringResolver dynamicConnectionStringResolver)
        {
            _telemetryDataSinkSetupServiceClient = telemetryDataSinkSetupServiceClient;
            _networkOperations = networkOperations;
            _serviceOperations = serviceOperations;
            _companyOperations = companyOperations;
            _dynamicConnectionStringResolver = dynamicConnectionStringResolver;
        }

        public IEnumerable<SinkInfo> GetSinksForNetwork(string networkId)
        {
            var list = new List<SinkInfo>();
            var allSinks = GetConfiguredSinks(networkId);

            var telemetryDataSinksMetadata = _telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata();

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

            var telemetryDataSinksMetadata = _telemetryDataSinkSetupServiceClient.GetTelemetryDataSinksMetadata();

            var workerSink = configuredSinks.SingleOrDefault(sink => String.Equals(sink.SinkName, sinkName, StringComparison.InvariantCultureIgnoreCase));
            if (workerSink == null)
                return null;

            var telemetryDataSinkMetadata = telemetryDataSinksMetadata.Incoming.Single(sink => String.Equals(sink.Name, sinkName, StringComparison.InvariantCultureIgnoreCase));

            var telemetryDataSink = (ITelemetryDataSink)Activator.CreateInstance(Type.GetType(telemetryDataSinkMetadata.TypeName));
            var allParameters = telemetryDataSinkMetadata.ParametersPresets.Union(workerSink.Parameters).ToDictionary(d => d.Key, d => d.Value);
            telemetryDataSink.Setup(_dynamicConnectionStringResolver, allParameters);

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