using System;
using System.Collections.Generic;
using NSubstitute;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Plugins.Core;
using Thriot.Reporting.Services.Dto;
using Thriot.Reporting.Services;

namespace Thriot.Reporting.Tests
{
    public abstract class NetworkTestBase
    {
        protected NetworkReportingService PrepareUnknownSinkReporting()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var telemetryDataSinkProcessor = Substitute.For<ITelemetryDataSinkProcessor>();

            deviceOperations.Get("1")
                .Returns(new Device
                {
                    Id = "1",
                    NetworkId = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    DeviceKey = "dk",
                    Name = "dn"
                });
            networkOperations.Get("2")
                .Returns(new Network
                {
                    Id = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    NetworkKey = "nk"
                });
            networkOperations.ListDevices("2").Returns(c => new List<Small> { new Small { Id = "1", Name = "dn" } });

            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "currentdata", SinkType = SinkType.CurrentData},
                new SinkInfo {SinkName = "timeseries", SinkType = SinkType.TimeSeries}
            });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2").Returns(c => null);
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("timeseries", "2").Returns(c => null);

            return new NetworkReportingService(telemetryDataSinkProcessor, networkOperations);
        }

        protected NetworkReportingService PrepareEmptyReporting()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var telemetryDataSinkProcessor = Substitute.For<ITelemetryDataSinkProcessor>();
            var currentDataSink = Substitute.For<ITelemetryDataSinkCurrent>();
            var timeSeriesSink = Substitute.For<ITelemetryDataSinkTimeSeries>();

            deviceOperations.Get("1")
                .Returns(new Device
                {
                    Id = "1",
                    NetworkId = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    DeviceKey = "dk",
                    Name = "dn"
                });
            networkOperations.Get("2")
                .Returns(new Network
                {
                    Id = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    NetworkKey = "nk"
                });
            networkOperations.ListDevices("2").Returns(c => new List<Small> { new Small { Id = "1", Name = "dn" } });

            currentDataSink.GetCurrentData(null).ReturnsForAnyArgs(c => new List<TelemetryData>());
            timeSeriesSink.GetTimeSeries(null, DateTime.UtcNow).ReturnsForAnyArgs(c => new List<TelemetryData>());

            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "currentdata", SinkType = SinkType.CurrentData},
                new SinkInfo {SinkName = "timeseries", SinkType = SinkType.TimeSeries}
            });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2").Returns(c => currentDataSink);
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("timeseries", "2").Returns(c => timeSeriesSink);

            return new NetworkReportingService(telemetryDataSinkProcessor, networkOperations);
        }

        protected NetworkReportingService PrepareSingleDeviceReporting()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var telemetryDataSinkProcessor = Substitute.For<ITelemetryDataSinkProcessor>();
            var currentDataSink = Substitute.For<ITelemetryDataSinkCurrent>();
            var timeSeriesSink = Substitute.For<ITelemetryDataSinkTimeSeries>();

            deviceOperations.Get("1")
                .Returns(new Device
                {
                    Id = "1",
                    NetworkId = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    DeviceKey = "dk",
                    Name = "dn"
                });
            networkOperations.Get("2")
                .Returns(new Network
                {
                    Id = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    NetworkKey = "nk",
                });
            networkOperations.ListDevices("2").Returns(c => new List<Small> { new Small { Id = "1", Name = "dn" } });

            currentDataSink.GetCurrentData(null).ReturnsForAnyArgs(c => new List<TelemetryData>
            {
                new TelemetryData("1", "{\"Temperature\": 24, \"Humidity\": 60}", DateTime.UtcNow)
            });
            timeSeriesSink.GetTimeSeries(null, DateTime.UtcNow).ReturnsForAnyArgs(c => new List<TelemetryData>
            {
                new TelemetryData("1", "{\"Temperature\": 24, \"Humidity\": 60}", DateTime.UtcNow),
                new TelemetryData("1", "{\"Temperature\": 25, \"Humidity\": 61}", DateTime.UtcNow)
            });

            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "currentdata", SinkType = SinkType.CurrentData},
                new SinkInfo {SinkName = "timeseries", SinkType = SinkType.TimeSeries}
            });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2").Returns(c => currentDataSink);
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("timeseries", "2").Returns(c => timeSeriesSink);

            return new NetworkReportingService(telemetryDataSinkProcessor, networkOperations);
        }

        protected NetworkReportingService PrepareMultiDeviceReporting()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var telemetryDataSinkProcessor = Substitute.For<ITelemetryDataSinkProcessor>();
            var currentDataSink = Substitute.For<ITelemetryDataSinkCurrent>();
            var timeSeriesSink = Substitute.For<ITelemetryDataSinkTimeSeries>();

            deviceOperations.Get("d1")
                .Returns(new Device
                {
                    Id = "d1",
                    NetworkId = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    DeviceKey = "dk",
                    Name = "dn"
                });
            deviceOperations.Get("d2")
                .Returns(new Device
                {
                    Id = "d2",
                    NetworkId = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    DeviceKey = "dk",
                    Name = "dn2"
                });
            networkOperations.Get("2")
                .Returns(new Network
                {
                    Id = "2",
                    ServiceId = "3",
                    CompanyId = "3",
                    NetworkKey = "nk",
                });
            networkOperations.ListDevices("2")
                .Returns(c => new List<Small> { new Small { Id = "d1", Name = "dn" }, new Small { Id = "d2", Name = "dn2" } });

            currentDataSink.GetCurrentData(null).ReturnsForAnyArgs(c => new List<TelemetryData>
            {
                new TelemetryData("d1", "{\"Temperature\": 24, \"Humidity\": 60}", DateTime.UtcNow),
                new TelemetryData("d2", "{\"Temperature\": 25, \"Humidity\": 61}", DateTime.UtcNow)
            });
            timeSeriesSink.GetTimeSeries(null, DateTime.UtcNow).ReturnsForAnyArgs(c => new List<TelemetryData>
            {
                new TelemetryData("d1", "{\"Temperature\": 24, \"Humidity\": 60}", DateTime.UtcNow.AddMinutes(-1)),
                new TelemetryData("d1", "{\"Temperature\": 24, \"Humidity\": 60}", DateTime.UtcNow),
                new TelemetryData("d2", "{\"Temperature\": 25, \"Humidity\": 61}", DateTime.UtcNow.AddMinutes(-2)),
                new TelemetryData("d2", "{\"Temperature\": 25, \"Humidity\": 61}", DateTime.UtcNow.AddMinutes(-1)),
                new TelemetryData("d2", "{\"Temperature\": 26, \"Humidity\": 62}", DateTime.UtcNow)
            });

            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "currentdata", SinkType = SinkType.CurrentData}
            });
            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "timeseries", SinkType = SinkType.TimeSeries}
            });

            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2").Returns(c => currentDataSink);
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("timeseries", "2").Returns(c => timeSeriesSink);

            return new NetworkReportingService(telemetryDataSinkProcessor, networkOperations);
        }
    }
}
