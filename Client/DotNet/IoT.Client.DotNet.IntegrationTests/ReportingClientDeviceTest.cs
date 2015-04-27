using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Client.DotNet.Management;
using Thriot.Client.DotNet.Platform;
using Thriot.Client.DotNet.Reporting;

namespace Thriot.Client.DotNet.IntegrationTests
{
    [TestClass]
    public class ReportingClientDeviceTest : TestBase
    {
        private string _deviceId;
        private string _deviceKey;

        [TestInitialize]
        public void TestInit()
        {
            RegisterDevice();
        }

        [TestMethod]
        public void GetSinksTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            Assert.AreEqual(2, sinks.Count());
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.Unauthorized)]
        public void GetSinksNotAuthTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, "asdfasdf");

            reportingClient.Device.GetSinks();
        }

        [TestMethod]
        public void CurrentDataSingleTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var onerecord = reportingClient.Device.GetCurrentData(sinks.First(s => s.SinkType == SinkType.CurrentData).SinkName);

            Assert.AreEqual(1, onerecord.Devices.Count);
            var device = onerecord.Devices[0];

            Assert.AreEqual(_deviceId, device.DeviceId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", device.Name);
            Assert.AreEqual("{\"Fld\":123}", device.Payload);
            Assert.IsTrue(device.Timestamp > 1000000);
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.Unauthorized)]
        public void CurrentDataAuthErrorTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Device.SetDevice(_deviceId, "1234");

            reportingClient.Device.GetCurrentData("nomatter");
        }

        [TestMethod]
        public void CurrentDataNoDataTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var onerecord = reportingClient.Device.GetCurrentData(sinks.First(s => s.SinkType == SinkType.CurrentData).SinkName);

            Assert.AreEqual(0, onerecord.Devices.Count);
        }

        [TestMethod]
        public void TimeSeriesSingleTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var onerecord = reportingClient.Device.GetTimeSeriesReport(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.AreEqual(1, onerecord.Devices.Count);
            var device = onerecord.Devices[0];

            Assert.AreEqual(_deviceId, device.DeviceId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", device.Name);
            Assert.AreEqual(1, device.Data.Count);
            var data = device.Data[0];
            Assert.AreEqual("{\"Fld\":123}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);
        }

        [TestMethod]
        public void TimeSeriesMultiTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");
            Thread.Sleep(100);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 124}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var onerecord = reportingClient.Device.GetTimeSeriesReport(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.AreEqual(1, onerecord.Devices.Count);
            var device = onerecord.Devices[0];

            Assert.AreEqual(_deviceId, device.DeviceId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", device.Name);
            Assert.AreEqual(2, device.Data.Count);
            var data = device.Data[0];
            Assert.AreEqual("{\"Fld\":123}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);
            data = device.Data[1];
            Assert.AreEqual("{\"Fld\":124}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.Unauthorized)]
        public void TimeSeriesAuthErrorTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Device.SetDevice(_deviceId, "1234");

            reportingClient.Device.GetTimeSeriesReport("nomatter", DateTime.UtcNow);
        }

        [TestMethod]
        public void TimeSeriesNoDataTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var onerecord = reportingClient.Device.GetTimeSeriesReport(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.AreEqual(1, onerecord.Devices.Count);
            var device = onerecord.Devices[0];
            Assert.AreEqual(_deviceId, device.DeviceId);
            Assert.AreEqual("árvíztűrő tükörfúrógép", device.Name);
            Assert.AreEqual(0, device.Data.Count);
        }

        [TestMethod]
        public void CurrentDataSingleCsvTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var record = reportingClient.Device.GetCurrentDataCsv(sinks.First(s => s.SinkType == SinkType.CurrentData).SinkName);
            Assert.IsTrue(record.StartsWith("DeviceId,Name,Time,Fld\r\n"));
            Assert.IsTrue(record.Contains("árvíztűrő"));
            Assert.IsTrue(record.Contains(_deviceId));
            Assert.IsTrue(record.Contains("123\r\n"));
        }

        [TestMethod]
        public void CurrentDataSingleCsvDataWithCommaTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"F,ld\": \"12,3\"}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var record = reportingClient.Device.GetCurrentDataCsv(sinks.First(s => s.SinkType == SinkType.CurrentData).SinkName);
            Assert.IsTrue(record.StartsWith("DeviceId,Name,Time,\"F,ld\"\r\n"));
            Assert.IsTrue(record.Contains("árvíztűrő"));
            Assert.IsTrue(record.Contains(_deviceId));
            Assert.IsTrue(record.Contains("\"12,3\"\r\n"));
        }

        [TestMethod]
        public void TimeSeriesMultiCsvTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _deviceKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123, \"A\": 234}");
            Thread.Sleep(100);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 124, \"B\": 456}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Device.SetDevice(_deviceId, _deviceKey);

            var sinks = reportingClient.Device.GetSinks();

            var record = reportingClient.Device.GetTimeSeriesReportCsv(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.IsTrue(record.StartsWith("DeviceId,Name,Time,Fld,A,B\r\n") || record.StartsWith("DeviceId,Name,Time,Fld,B,A\r\n"));
            Assert.IsTrue(record.Contains("árvíztűrő"));
            Assert.IsTrue(record.Contains(_deviceId));
            Assert.IsTrue(record.Contains("123,234,\r\n"));
            Assert.IsTrue(record.Contains("124,,456\r\n"));
        }

        private void RegisterDevice()
        {
            var managementClient = new ManagementClient(ManagementApi);

            managementClient.User.Register(new Register
            {
                Email = Guid.NewGuid() + "@test.hu",
                Name = "test user",
                Password = "p@ssw0rd"
            });

            var companyId = managementClient.Company.Create(new Company { Name = "company" });
            var serviceId = managementClient.Service.Create(new Service { CompanyId = companyId, Name = "service" });
            var networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

            var messageSinkParameters = new List<TelemetryDataSinkParameters>
            {
                new TelemetryDataSinkParameters
                {
                    SinkName = SinkData,
                    Parameters = new Dictionary<string, string>()
                },
                new TelemetryDataSinkParameters
                {
                    SinkName = SinkTimeSeries,
                    Parameters = new Dictionary<string, string>()
                }
            };

            managementClient.Service.UpdateIncomingTelemetryDataSinks(serviceId, messageSinkParameters);

            _deviceId = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "árvíztűrő tükörfúrógép" });

            _deviceKey = managementClient.Device.Get(_deviceId).DeviceKey;
        }
    }
}
