using System;
using System.Collections.Generic;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Plugins.Core;
using IoT.Reporting.Dto;
using IoT.Reporting.Services;
using NSubstitute;

namespace IoT.Reporting.Tests
{
    public abstract class DeviceTestBase
    {
        protected DeviceReportingService PrepareUnknownSinkReporting()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
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


            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "currentdata", SinkType = SinkType.CurrentData},
                new SinkInfo {SinkName = "timeseries", SinkType = SinkType.TimeSeries}
            });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2").Returns(c => null);
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("timeseries", "2").Returns(c => null);

            return new DeviceReportingService(telemetryDataSinkProcessor, deviceOperations);
        }

        protected DeviceReportingService PrepareEmptyDeviceReporting()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
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

            currentDataSink.GetCurrentData(null).ReturnsForAnyArgs(c => new List<TelemetryData>());
            timeSeriesSink.GetTimeSeries(null, DateTime.UtcNow).ReturnsForAnyArgs(c => new List<TelemetryData>());

            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "currentdata", SinkType = SinkType.CurrentData},
                new SinkInfo {SinkName = "timeseries", SinkType = SinkType.TimeSeries}
            });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2").Returns(c => currentDataSink);
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("timeseries", "2").Returns(c => timeSeriesSink);

            return new DeviceReportingService(telemetryDataSinkProcessor, deviceOperations);
        }

        protected DeviceReportingService PrepareSingleDeviceReporting()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
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

            currentDataSink.GetCurrentData(null).ReturnsForAnyArgs(c => new List<TelemetryData>
            {
                new TelemetryData("1", "{\"Temperature\": 24, \"Humidity\": 60}", DateTime.UtcNow)
            });
            timeSeriesSink.GetTimeSeries(null, DateTime.UtcNow).ReturnsForAnyArgs(c => new List<TelemetryData>
            {
                new TelemetryData("1", "{\"Temperature\": 24, \"Humidity\": 60}", DateTime.UtcNow.AddMinutes(-1)),
                new TelemetryData("1", "{\"Temperature\": 25, \"Humidity\": 61}", DateTime.UtcNow)
            });
            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
            {
                new SinkInfo {SinkName = "currentdata", SinkType = SinkType.CurrentData},
                new SinkInfo {SinkName = "timeseries", SinkType = SinkType.TimeSeries}
            });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2").Returns(c => currentDataSink);
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("timeseries", "2").Returns(c => timeSeriesSink);

            return new DeviceReportingService(telemetryDataSinkProcessor, deviceOperations);
        }
    }
}
