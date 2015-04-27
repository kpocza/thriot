using System;
using System.Collections.Generic;
using System.Linq;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Plugins.Core;
using Thriot.Reporting.Dto;

namespace Thriot.Reporting.Services
{
    public class DeviceReportingService
    {
        private readonly ITelemetryDataSinkProcessor _telemetryDataSinkProcessor;
        private readonly IDeviceOperations _deviceOperations;

        public DeviceReportingService(ITelemetryDataSinkProcessor telemetryDataSinkProcessor, IDeviceOperations deviceOperations)
        {
            _telemetryDataSinkProcessor = telemetryDataSinkProcessor;
            _deviceOperations = deviceOperations;
        }

        public IEnumerable<SinkInfoDto> GetSinks(string deviceId)
        {
            var device = _deviceOperations.Get(deviceId);

            var list = _telemetryDataSinkProcessor.GetSinksForNetwork(device.NetworkId);

            return list.Select(s => new SinkInfoDto { SinkName = s.SinkName, SinkType = s.SinkType }).ToList();
        }

        public CurrentDataReportDto CurrentDataStructuredReport(SinkAndDeviceDto sinkAndDevice)
        {
            var device = _deviceOperations.Get(sinkAndDevice.DeviceId);
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndDevice.SinkName, device.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = new[] { new Small { Id = device.Id, Name = device.Name } };
            var telemetryDataList = ((ITelemetryDataSinkCurrent)telemetryDataSink).GetCurrentData(devices.Select(d => d.Id));

            return StructuredDtoConverters.CurrentDataReport(devices, telemetryDataList);
        }

        public TimeSeriesReportDto TimeSeriesStructuredReport(SinkAndDeviceDto sinkAndDevice, DateTime date)
        {
            var device = _deviceOperations.Get(sinkAndDevice.DeviceId);
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndDevice.SinkName, device.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = new[] { new Small { Id = device.Id, Name = device.Name } };
            var telemetryDataList = ((ITelemetryDataSinkTimeSeries)telemetryDataSink).GetTimeSeries(devices.Select(d => d.Id), date.Date);

            return StructuredDtoConverters.TimeSeriesReport(devices, telemetryDataList);
        }

        public FlatReportDto CurrentDataFlatReport(SinkAndDeviceDto sinkAndDevice)
        {
            var device = _deviceOperations.Get(sinkAndDevice.DeviceId);
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndDevice.SinkName, device.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = new[] { new Small { Id = device.Id, Name = device.Name } };
            var telemetryDataList = ((ITelemetryDataSinkCurrent)telemetryDataSink).GetCurrentData(devices.Select(d => d.Id));

            return FlatDtoConverters.CurrentDataReport(devices, telemetryDataList);
        }

        public FlatReportDto TimeSeriesFlatReport(SinkAndDeviceDto sinkAndDevice, DateTime date)
        {
            var device = _deviceOperations.Get(sinkAndDevice.DeviceId);
            var telemetryDataSink = _telemetryDataSinkProcessor.WorkerTelemetryDataSink(sinkAndDevice.SinkName, device.NetworkId);
            if (telemetryDataSink == null)
                return null;

            var devices = new[] { new Small { Id = device.Id, Name = device.Name } };
            var telemetryDataList = ((ITelemetryDataSinkTimeSeries)telemetryDataSink).GetTimeSeries(devices.Select(d => d.Id), date.Date);

            return FlatDtoConverters.TimeSeriesReport(devices, telemetryDataList);
        }
    }
}
