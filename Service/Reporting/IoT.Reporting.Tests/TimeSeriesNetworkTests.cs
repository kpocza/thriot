using System;
using System.Linq;
using IoT.Reporting.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Reporting.Tests
{
    [TestClass]
    public class TimeSeriesNetworkTests : NetworkTestBase
    {
        [TestMethod]
        public void UnknownSinkStructuredTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.TimeSeriesStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceReportingEmptyStructuredTest()
        {
            var reportingService = PrepareEmptyReporting();

            var result = reportingService.TimeSeriesStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(1, result.Devices.Count);
            Assert.AreEqual("1", result.Devices.First().DeviceId);
            Assert.AreEqual("dn", result.Devices.First().Name);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsStructuredTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.TimeSeriesStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(1, result.Devices.Count);
            var record = result.Devices.First();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.AreEqual(2, record.Data.Count);
            Assert.AreEqual("{\"Temperature\": 24, \"Humidity\": 60}", record.Data.First().Payload.ToString());
            Assert.IsTrue(record.Data.First().Timestamp > 10000);
        }

        [TestMethod]
        public void MultiDeviceReportingHasRecordsStructuredTest()
        {
            var reportingService = PrepareMultiDeviceReporting();

            var result = reportingService.TimeSeriesStructuredReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(2, result.Devices.Count);
            var record = result.Devices.First();
            Assert.AreEqual("d1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.AreEqual(2, record.Data.Count);
            var first = record.Data.First();
            var last = record.Data.Last();
            Assert.AreEqual("{\"Temperature\": 24, \"Humidity\": 60}", first.Payload.ToString());
            Assert.AreEqual("{\"Temperature\": 24, \"Humidity\": 60}", last.Payload.ToString());
            Assert.IsTrue(first.Timestamp < last.Timestamp);

            record = result.Devices.Last();
            Assert.AreEqual("d2", record.DeviceId);
            Assert.AreEqual("dn2", record.Name);
            Assert.AreEqual(3, record.Data.Count);
            first = record.Data.First();
            last = record.Data.Last();
            Assert.AreEqual("{\"Temperature\": 25, \"Humidity\": 61}", first.Payload.ToString());
            Assert.AreEqual("{\"Temperature\": 26, \"Humidity\": 62}", last.Payload.ToString());
            Assert.IsTrue(first.Timestamp < last.Timestamp);
        }

        [TestMethod]
        public void UnknownSinkFlatTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.TimeSeriesFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceReportingEmptyFlatTest()
        {
            var reportingService = PrepareEmptyReporting();

            var result = reportingService.TimeSeriesFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(0, result.Properties.Count);
            Assert.AreEqual(0, result.Rows.Count);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsFlatTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.TimeSeriesFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(2, result.Properties.Count);
            Assert.AreEqual("Temperature", result.Properties[0]);
            Assert.AreEqual("Humidity", result.Properties[1]);

            Assert.AreEqual(2, result.Rows.Count);
            var record = result.Rows.First();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.AreEqual("24", record.Fields.Single(f => f.Key == "Temperature").Value);
            Assert.AreEqual("60", record.Fields.Single(f => f.Key == "Humidity").Value);
            Assert.IsTrue(record.Timestamp > 10000);
            record = result.Rows.Last();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.AreEqual("25", record.Fields.Single(f => f.Key == "Temperature").Value);
            Assert.AreEqual("61", record.Fields.Single(f => f.Key == "Humidity").Value);
            Assert.IsTrue(record.Timestamp > 10000);
        }

        [TestMethod]
        public void MultiDeviceReportingHasRecordsFlatTest()
        {
            var reportingService = PrepareMultiDeviceReporting();

            var result = reportingService.TimeSeriesFlatReport(new SinkAndNetworkDto { NetworkId = "2", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(2, result.Properties.Count);
            Assert.AreEqual("Temperature", result.Properties[0]);
            Assert.AreEqual("Humidity", result.Properties[1]);

            Assert.AreEqual(5, result.Rows.Count);
            var record = result.Rows.First();
            Assert.AreEqual("d1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            record = result.Rows.Last();
            Assert.AreEqual("d2", record.DeviceId);
            Assert.AreEqual("dn2", record.Name);
        }
    }
}
