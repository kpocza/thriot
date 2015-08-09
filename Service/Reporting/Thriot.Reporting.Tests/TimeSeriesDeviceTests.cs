using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Reporting.Services.Dto;

namespace Thriot.Reporting.Tests
{
    [TestClass]
    public class TimeSeriesDeviceTests : DeviceTestBase
    {
        [TestMethod]
        public void UnknownSinkStructuredTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.TimeSeriesStructuredReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceReportingEmptyStructuredTest()
        {
            var reportingService = PrepareEmptyDeviceReporting();

            var result = reportingService.TimeSeriesStructuredReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(1, result.Devices.Count);
            Assert.AreEqual(0, result.Devices[0].Data.Count);
            Assert.AreEqual("1", result.Devices[0].DeviceId);
            Assert.AreEqual("dn", result.Devices[0].Name);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsStructuredTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.TimeSeriesStructuredReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(1, result.Devices.Count);
            var record = result.Devices.First();
            Assert.AreEqual("1", record.DeviceId);
            Assert.AreEqual("dn", record.Name);
            Assert.AreEqual(2, record.Data.Count);
            Assert.AreEqual("{\"Temperature\": 24, \"Humidity\": 60}", record.Data.First().Payload);
            Assert.IsTrue(record.Data.First().Timestamp > 10000);
            Assert.AreEqual("{\"Temperature\": 25, \"Humidity\": 61}", record.Data.Last().Payload);
            Assert.IsTrue(record.Data.Last().Timestamp > 10000);
        }

        [TestMethod]
        public void UnknownSinkFlatTest()
        {
            var reportingService = PrepareUnknownSinkReporting();

            var result = reportingService.TimeSeriesFlatReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.IsNull(result);
        }


        [TestMethod]
        public void SingleDeviceReportingEmptyFlatTest()
        {
            var reportingService = PrepareEmptyDeviceReporting();

            var result = reportingService.TimeSeriesFlatReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "timeseries" }, DateTime.UtcNow);
            Assert.AreEqual(0, result.Properties.Count);
            Assert.AreEqual(0, result.Rows.Count);
        }

        [TestMethod]
        public void SingleDeviceReportingHasRecordsFlatTest()
        {
            var reportingService = PrepareSingleDeviceReporting();

            var result = reportingService.TimeSeriesFlatReport(new SinkAndDeviceDto { DeviceId = "1", SinkName = "timeseries" }, DateTime.UtcNow);
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
    }
}
