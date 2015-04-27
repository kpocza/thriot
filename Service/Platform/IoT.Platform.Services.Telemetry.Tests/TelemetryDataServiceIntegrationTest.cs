using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Thriot.Plugins.Core;

namespace Thriot.Platform.Services.Telemetry.Tests
{
    [TestClass]
    public class TelemetryDataServiceIntegrationTest : TestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void IncomingMessageTest()
        {
            IncomingStubs.Initialize(_deviceId);

            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            telemetryDataSinkResolver.ResolveIncoming(_deviceId)
                .Returns(new List<ITelemetryDataSink>() { new IncomingStubs.CurrentDataStub(), new IncomingStubs.TimeSeriesStub() });

            var telemetryDataService = new TelemetryDataService(telemetryDataSinkResolver);

            telemetryDataService.RecordTelemetryData(_deviceId, JToken.Parse("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));

            Assert.AreEqual(2, IncomingStubs.RecordCounter);
        }
    }
}
