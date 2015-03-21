using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using IoT.Client.DotNet.Management;
using IoT.Client.DotNet.Platform;
using IoT.Client.DotNet.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Client.DotNet.IntegrationTests
{
    [TestClass]
    public class ReportingClientNetworkTest : TestBase
    {
        private string _deviceId1;
        private string _deviceId2;
        private string _networkId;
        private string _networkKey;

        [TestInitialize]
        public void TestInit()
        {
            RegisterNetworkAndDevices();
        }

        [TestMethod]
        public void GetSinksTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            Assert.AreEqual(2, sinks.Count());
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.Unauthorized)]
        public void GetSinksNotAuthTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, "asdfasdf");

            reportingClient.Network.GetSinks();
        }

        [TestMethod]
        public void CurrentDataSingleTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");
            ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId2, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 124}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var onerecord = reportingClient.Network.GetCurrentData(sinks.First(s => s.SinkType == SinkType.CurrentData).SinkName);

            Assert.AreEqual(2, onerecord.Devices.Count);
            var device = onerecord.Devices.Single(d => d.DeviceId == _deviceId1);

            Assert.AreEqual(_deviceId1, device.DeviceId);
            Assert.AreEqual("d1", device.Name);
            Assert.AreEqual("{\"Fld\":123}", device.Payload);
            Assert.IsTrue(device.Timestamp > 1000000);

            device = onerecord.Devices.Single(d => d.DeviceId == _deviceId2);
            Assert.AreEqual(_deviceId2, device.DeviceId);
            Assert.AreEqual("d2", device.Name);
            Assert.AreEqual("{\"Fld\":124}", device.Payload);
            Assert.IsTrue(device.Timestamp > 1000000);
        }

        [TestMethod]
        [ExpectedHttpStatusCodeAttribute(HttpStatusCode.Unauthorized)]
        public void CurrentDataAuthErrorTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Network.SetNetwork(_networkId, "1234");

            reportingClient.Network.GetCurrentData("nomatter");
        }

        [TestMethod]
        public void CurrentDataNoDataTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var onerecord = reportingClient.Network.GetCurrentData(sinks.First(s => s.SinkType == SinkType.CurrentData).SinkName);

            Assert.AreEqual(0, onerecord.Devices.Count);
        }

        [TestMethod]
        public void TimeSeriesSingleTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var onerecord = reportingClient.Network.GetTimeSeriesReport(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.AreEqual(2, onerecord.Devices.Count);
            var device = onerecord.Devices.Single(d => d.DeviceId == _deviceId1);

            Assert.AreEqual(_deviceId1, device.DeviceId);
            Assert.AreEqual("d1", device.Name);
            Assert.AreEqual(1, device.Data.Count);
            var data = device.Data[0];
            Assert.AreEqual("{\"Fld\":123}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);

            device = onerecord.Devices.Single(d => d.DeviceId == _deviceId2);
            Assert.AreEqual(0, device.Data.Count);
        }

        [TestMethod]
        public void TimeSeriesMultiTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");
            Thread.Sleep(100);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 124}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var onerecord = reportingClient.Network.GetTimeSeriesReport(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.AreEqual(2, onerecord.Devices.Count);
            var device = onerecord.Devices.Single(d => d.DeviceId == _deviceId1);

            Assert.AreEqual(_deviceId1, device.DeviceId);
            Assert.AreEqual("d1", device.Name);
            Assert.AreEqual(2, device.Data.Count);
            var data = device.Data[0];
            Assert.AreEqual("{\"Fld\":123}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);
            data = device.Data[1];
            Assert.AreEqual("{\"Fld\":124}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);

            device = onerecord.Devices.Single(d => d.DeviceId == _deviceId2);
            Assert.AreEqual(0, device.Data.Count);
        }

        [TestMethod]
        public void TimeSeriesMultiMultiTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");
            Thread.Sleep(100);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 124}");
            ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId2, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 125}");
            Thread.Sleep(100);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 126}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var onerecord = reportingClient.Network.GetTimeSeriesReport(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.AreEqual(2, onerecord.Devices.Count);
            var device = onerecord.Devices.Single(d => d.DeviceId == _deviceId1);

            Assert.AreEqual(_deviceId1, device.DeviceId);
            Assert.AreEqual("d1", device.Name);
            Assert.AreEqual(2, device.Data.Count);
            var data = device.Data[0];
            Assert.AreEqual("{\"Fld\":123}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);
            data = device.Data[1];
            Assert.AreEqual("{\"Fld\":124}", data.Payload);
            Assert.IsTrue(data.Timestamp > 1000000);

            device = onerecord.Devices.Single(d => d.DeviceId == _deviceId2);
            Assert.AreEqual(_deviceId2, device.DeviceId);
            Assert.AreEqual(2, device.Data.Count);
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.Unauthorized)]
        public void TimeSeriesAuthErrorTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123}");

            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Network.SetNetwork(_networkId, "1234");

            reportingClient.Network.GetTimeSeriesReport("nomatter", DateTime.UtcNow);
        }

        [TestMethod]
        public void TimeSeriesNoDataTest()
        {
            var reportingClient = new ReportingClient(ReportingApi);
            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var onerecord = reportingClient.Network.GetTimeSeriesReport(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.AreEqual(2, onerecord.Devices.Count);
            var device = onerecord.Devices.Single(d => d.DeviceId == _deviceId1);
            Assert.AreEqual(_deviceId1, device.DeviceId);
            Assert.AreEqual("d1", device.Name);
            Assert.AreEqual(0, device.Data.Count);
            device = onerecord.Devices.Single(d => d.DeviceId == _deviceId2);
            Assert.AreEqual(_deviceId2, device.DeviceId);
            Assert.AreEqual("d2", device.Name);
            Assert.AreEqual(0, device.Data.Count);
        }


        [TestMethod]
        public void CurrentDataSingleCsvTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123, \"A\": 234}");
            ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId2, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 124, \"B\": 456}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var record = reportingClient.Network.GetCurrentDataCsv(sinks.First(s => s.SinkType == SinkType.CurrentData).SinkName);
            Assert.IsTrue(record.StartsWith("DeviceId,Name,Time,Fld,A,B\r\n") || record.StartsWith("DeviceId,Name,Time,Fld,B,A\r\n"));
            Assert.IsTrue(record.Contains("d1"));
            Assert.IsTrue(record.Contains("d2"));
            Assert.IsTrue(record.Contains(_deviceId1));
            Assert.IsTrue(record.Contains(_deviceId2));
            Assert.IsTrue(record.Contains("123,234,\r\n") || record.Contains("123,,234\r\n"));
            Assert.IsTrue(record.Contains("124,,456\r\n") || record.Contains("124,456,\r\n"));
        }

        [TestMethod]
        public void TimeSeriesMultiMultiCsvTest()
        {
            var ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId1, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 123, \"A\": 234}");
            Thread.Sleep(100);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 124, \"A\": 345}");
            ocassionalConnectionClient = new OccassionalConnectionClient(PlatformApi, _deviceId2, _networkKey);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 125, \"B\": 567}");
            Thread.Sleep(100);
            ocassionalConnectionClient.RecordTelmetryData("{\"Fld\": 126, \"B\": 678}");

            var reportingClient = new ReportingClient(ReportingApi);

            reportingClient.Network.SetNetwork(_networkId, _networkKey);

            var sinks = reportingClient.Network.GetSinks();

            var record = reportingClient.Network.GetTimeSeriesReportCsv(sinks.First(s => s.SinkType == SinkType.TimeSeries).SinkName, DateTime.UtcNow);

            Assert.IsTrue(record.StartsWith("DeviceId,Name,Time,Fld,A,B\r\n") || record.StartsWith("DeviceId,Name,Time,Fld,B,A\r\n"));
            Assert.IsTrue(record.Contains("d1"));
            Assert.IsTrue(record.Contains("d2"));
            Assert.IsTrue(record.Contains(_deviceId1));
            Assert.IsTrue(record.Contains(_deviceId2));
            Assert.IsTrue(record.Contains("123,234,\r\n") || record.Contains("123,,234\r\n"));
            Assert.IsTrue(record.Contains("124,345,\r\n") || record.Contains("124,,345\r\n"));
            Assert.IsTrue(record.Contains("125,,567\r\n") || record.Contains("125,567,\r\n"));
            Assert.IsTrue(record.Contains("126,,678\r\n") || record.Contains("126,678,\r\n"));
        }

        private void RegisterNetworkAndDevices()
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
            _networkId = managementClient.Network.Create(new Network { CompanyId = companyId, ServiceId = serviceId, Name = "árvíztűrő tükörfúrógép" });

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

            _deviceId1 = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = _networkId, Name = "d1" });
            _deviceId2 = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = _networkId, Name = "d2" });

            _networkKey = managementClient.Network.Get(_networkId).NetworkKey;
        }
    }
}
