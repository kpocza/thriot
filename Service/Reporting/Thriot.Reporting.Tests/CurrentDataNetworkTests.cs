using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Reporting.Services.Dto;

namespace Thriot.Reporting.Tests
{
    [TestClass]
    public class CurrentDataNetworkTests : NetworkTestBase
    {
        [TestMethod]
        public void UnknownSinkStructuredTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.CurrentDataStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceReportingEmptyStructuredTest()
        {
            var reportingService = PrepareEmptyReporting();

            var result = reportingService.CurrentDataStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.AreEqual(0, result.Devices.Count);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsStructuredTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.CurrentDataStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.AreEqual(1, result.Devices.Count);
            var record = result.Devices.First();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.IsTrue(record.Timestamp > 100000);
            Assert.AreEqual("{\"Temperature\": 24, \"Humidity\": 60}", record.Payload.ToString());
        }

        [TestMethod]
        public void MultiDeviceReportingHasRecordsStructuredTest()
        {
            var reportingService = PrepareMultiDeviceReporting();

            var result = reportingService.CurrentDataStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.AreEqual(2, result.Devices.Count);
            var record = result.Devices.First();
            Assert.AreEqual("d1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.IsTrue(record.Timestamp > 100000);
            Assert.AreEqual("{\"Temperature\": 24, \"Humidity\": 60}", record.Payload.ToString());
            record = result.Devices.Last();
            Assert.AreEqual("d2", record.DeviceId);
            Assert.AreEqual("dn2", record.Name);
            Assert.IsTrue(record.Timestamp > 100000);
            Assert.AreEqual("{\"Temperature\": 25, \"Humidity\": 61}", record.Payload.ToString());
        }

        [TestMethod]
        public void UnknownSinkFlatTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.CurrentDataFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceReportingEmptyFlatTest()
        {
            var reportingService = PrepareEmptyReporting();

            var result = reportingService.CurrentDataFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.AreEqual(0, result.Rows.Count);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsFlatTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.CurrentDataFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.AreEqual(2, result.Properties.Count);
            Assert.AreEqual("Temperature", result.Properties[0]);
            Assert.AreEqual("Humidity", result.Properties[1]);
            Assert.AreEqual(1, result.Rows.Count);
            var record = result.Rows.First();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.IsTrue(record.Timestamp > 100000);
            Assert.AreEqual("24", record.Fields.Single(f => f.Key == "Temperature").Value);
            Assert.AreEqual("60", record.Fields.Single(f => f.Key == "Humidity").Value);
        }

        [TestMethod]
        public void MultiDeviceReportingHasRecordsFlatTest()
        {
            var reportingService = PrepareMultiDeviceReporting();

            var result = reportingService.CurrentDataFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "currentdata" });
            Assert.AreEqual(2, result.Properties.Count);
            Assert.AreEqual("Temperature", result.Properties[0]);
            Assert.AreEqual("Humidity", result.Properties[1]);
            var record = result.Rows.First();
            Assert.AreEqual("d1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.AreEqual("24", record.Fields.Single(f => f.Key == "Temperature").Value);
            Assert.AreEqual("60", record.Fields.Single(f => f.Key == "Humidity").Value);
            record = result.Rows.Last();
            Assert.AreEqual("d2", record.DeviceId);
            Assert.AreEqual("dn2", record.Name);
            Assert.AreEqual("25", record.Fields.Single(f => f.Key == "Temperature").Value);
            Assert.AreEqual("61", record.Fields.Single(f => f.Key == "Humidity").Value);
        }
    }
}
