using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.Reporting.Dto;
using Thriot.Reporting.Services;

namespace Thriot.Reporting.Tests
{
    [TestClass]
    public class GetSinksTests
    {
        [TestMethod]
        public void GetSinksForNetworkTest()
        {
            var telemetryDataSinkProcessor = Substitute.For<ITelemetryDataSinkProcessor>();

            telemetryDataSinkProcessor.GetSinksForNetwork("2").Returns(c => new List<SinkInfo>
                {
                    new SinkInfo { SinkName = "currentdata", SinkType = SinkType.CurrentData}
                });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2")
                .Returns(c => new IncomingStubs.CurrentDataStub());

            var reportingService = new NetworkReportingService(telemetryDataSinkProcessor, null);

            var sinks = reportingService.GetSinks("2");

            Assert.AreEqual(1, sinks.Count());
            Assert.AreEqual("currentdata", sinks.First().SinkName);
            Assert.AreEqual(SinkType.CurrentData, sinks.First().SinkType);
        }

        [TestMethod]
        public void GetSinksForDeviceTest()
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
                    new SinkInfo { SinkName = "currentdata", SinkType = SinkType.CurrentData}
                });
            telemetryDataSinkProcessor.WorkerTelemetryDataSink("currentdata", "2")
                .Returns(c => new IncomingStubs.CurrentDataStub());

            var reportingService = new DeviceReportingService(telemetryDataSinkProcessor, deviceOperations);

            var sinks = reportingService.GetSinks("1");

            Assert.AreEqual(1, sinks.Count());
            Assert.AreEqual("currentdata", sinks.First().SinkName);
            Assert.AreEqual(SinkType.CurrentData, sinks.First().SinkType);
        }
    }
}
