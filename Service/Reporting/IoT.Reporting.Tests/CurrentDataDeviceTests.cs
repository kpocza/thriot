using System.Linq;
using IoT.Reporting.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Reporting.Tests
{
    [TestClass]
    public class CurrentDataDeviceTests : DeviceTestBase
    {
        [TestMethod]
        public void UnknownSinkStructuredTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.CurrentDataStructuredReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "currentdata" });
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceReportingEmptyStructuredTest()
        {
            var reportingService = PrepareEmptyDeviceReporting();

            var result = reportingService.CurrentDataStructuredReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "currentdata" });
            Assert.AreEqual(0, result.Devices.Count);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsStructuredTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.CurrentDataStructuredReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "currentdata" });
            Assert.AreEqual(1, result.Devices.Count);
            var record = result.Devices.First();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("{\"Temperature\": 24, \"Humidity\": 60}", record.Payload.ToString());
            Assert.IsTrue(record.Timestamp > 1000000);
        }

        [TestMethod]
        public void UnknownSinkFlatTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.CurrentDataFlatReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "currentdata" });
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceReportingEmptyFlatTest()
        {
            var reportingService = PrepareEmptyDeviceReporting();

            var result = reportingService.CurrentDataFlatReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "currentdata" });
            Assert.AreEqual(0, result.Properties.Count);
            Assert.AreEqual(0, result.Rows.Count);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsFlatTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.CurrentDataFlatReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "currentdata" });
            Assert.AreEqual(2, result.Properties.Count);
            Assert.AreEqual("Temperature", result.Properties[0]);
            Assert.AreEqual("Humidity", result.Properties[1]);

            var record = result.Rows.Single();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.IsTrue(record.Timestamp > 1000000);
            Assert.AreEqual(2, record.Fields.Count);
            Assert.AreEqual("24", record.Fields.Single(f => f.Key == "Temperature").Value);
            Assert.AreEqual("60", record.Fields.Single(f => f.Key == "Humidity").Value);
        }
    }
}
