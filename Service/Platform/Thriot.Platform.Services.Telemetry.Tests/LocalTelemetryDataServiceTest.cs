using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Platform.Services.Telemetry.Recording;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class LocalTelemetryDataServiceTest : TestBase
    {
        [TestMethod]
        public void IncomingTelemetryDataTest()
        {
            var deviceOperations = Substitute.For<Management.Model.Operations.IDeviceOperations>();

            deviceOperations.Create(null).ReturnsForAnyArgs("12345");

            IncomingStubs.Initialize("12345");

            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            var device1 = new Management.Model.Device()
            {
                Network = new Management.Model.Network() { Id = "1234" },
                Service = new Management.Model.Service() { Id = "2345" },
                Company = new Management.Model.Company() { Id = "3456" },
                Name = "new device",
                DeviceKey = Identity.Next()
            };

            var deviceId = deviceOperations.Create(device1);

            telemetryDataSinkResolver.ResolveIncoming(deviceId)
                .Returns(new List<ITelemetryDataSink>() { new IncomingStubs.CurrentDataStub(), new IncomingStubs.TimeSeriesStub() });

            var telemetryDataService = new DirectTelemetryDataService(telemetryDataSinkResolver);

            telemetryDataService.RecordTelemetryData(deviceId, JToken.Parse("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));

            Assert.AreEqual(2, IncomingStubs.RecordCounter);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void IncomingTelemetryDataNotAuthDeviceTest()
        {
            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            var telemetryDataService = new DirectTelemetryDataService(telemetryDataSinkResolver);

            telemetryDataService.RecordTelemetryData(null, JToken.Parse("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IncomingTelemetryDataNullPayloadTest()
        {
            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            var telemetryDataService = new DirectTelemetryDataService(telemetryDataSinkResolver);

            telemetryDataService.RecordTelemetryData("1234", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IncomingTelemetryDataLongPayloadTest()
        {
            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            var telemetryDataService = new DirectTelemetryDataService(telemetryDataSinkResolver);

            telemetryDataService.RecordTelemetryData("1234", JToken.Parse("{\"LongString\": \"" + string.Join(",", Enumerable.Range(0, 1000)) + "\"}"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IncomingTelemetryDataNotSinkTest()
        {
            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            telemetryDataSinkResolver.ResolveIncoming("1234")
                .Returns(new List<ITelemetryDataSink>());

            var telemetryDataService = new DirectTelemetryDataService(telemetryDataSinkResolver);

            telemetryDataService.RecordTelemetryData("1234", JToken.Parse("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
        }

        [TestMethod]
        public void IncomingTelemetryDataRealDeviceTest()
        {
            Initialize();

            IncomingStubs.Initialize(_deviceId);

            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            telemetryDataSinkResolver.ResolveIncoming(_deviceId)
                .Returns(new List<ITelemetryDataSink>() { new IncomingStubs.CurrentDataStub(), new IncomingStubs.TimeSeriesStub() });

            var telemetryDataService = new DirectTelemetryDataService(telemetryDataSinkResolver);

            telemetryDataService.RecordTelemetryData(_deviceId, JToken.Parse("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));

            Assert.AreEqual(2, IncomingStubs.RecordCounter);
        }
    }
}
