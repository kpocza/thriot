using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Objects.Model.Operations;
using Thriot.Plugins.Core;
using Thriot.Reporting.Dto;

namespace Thriot.Reporting.Services
{
    public class NetworkReportingService
    {
        private readonly ITelemetryDataSinkProcessor _telemetryDataSinkProcessor;
        private readonly INetworkOperations _networkOperations;

        public NetworkReportingService(ITelemetryDataSinkProcessor telemetryDataSinkProcessor, INetworkOperations networkOperations)
        {
            _telemetryDataSinkProcessor = telemetryDataSinkProcessor;
            _networkOperations = networkOperations;
        }

        public IEnumerable<SinkInfoDto> GetSinks(string networkId)
        {
            var list = _telemetryDataSinkProcessor.GetSinksForNetwork(networkId);

            return list.Select(s => new SinkInfoDto {SinkName = s.SinkName, SinkType = s.SinkType}).ToList();
        }

        public CurrentDataReportDto CurrentDataStructuredReport(SinkAndNetworkDto sinkAndNetwork)
        {
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndNetwork.SinkName, sinkAndNetwork.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = _networkOperations.ListDevices(sinkAndNetwork.NetworkId);
            var telemetryDataList = ((ITelemetryDataSinkCurrent)telemetryDataSink).GetCurrentData(devices.Select(d => d.Id));

            return StructuredDtoConverters.CurrentDataReport(devices, telemetryDataList);
        }

        public TimeSeriesReportDto TimeSeriesStructuredReport(SinkAndNetworkDto sinkAndNetwork, DateTime date)
        {
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndNetwork.SinkName, sinkAndNetwork.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = _networkOperations.ListDevices(sinkAndNetwork.NetworkId);
            var telemetryDataList = ((ITelemetryDataSinkTimeSeries)telemetryDataSink).GetTimeSeries(devices.Select(d => d.Id), date.Date);

            return StructuredDtoConverters.TimeSeriesReport(devices, telemetryDataList);
        }

        public FlatReportDto CurrentDataFlatReport(SinkAndNetworkDto sinkAndNetwork)
        {
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndNetwork.SinkName, sinkAndNetwork.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = _networkOperations.ListDevices(sinkAndNetwork.NetworkId);
            var telemetryDataList = ((ITelemetryDataSinkCurrent)telemetryDataSink).GetCurrentData(devices.Select(d => d.Id));

            return FlatDtoConverters.CurrentDataReport(devices, telemetryDataList);
        }

        public FlatReportDto TimeSeriesFlatReport(SinkAndNetworkDto sinkAndNetwork, DateTime date)
        {
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndNetwork.SinkName, sinkAndNetwork.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = _networkOperations.ListDevices(sinkAndNetwork.NetworkId);
            var telemetryDataList = ((ITelemetryDataSinkTimeSeries)telemetryDataSink).GetTimeSeries(devices.Select(d => d.Id), date.Date);

            return FlatDtoConverters.TimeSeriesReport(devices, telemetryDataList);
        }
    }
}
