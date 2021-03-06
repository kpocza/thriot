﻿using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Client.DotNet.Management;
using Thriot.Client.DotNet.Platform;

namespace Thriot.Client.DotNet.IntegrationTests
{
    [TestClass]
    public class OccasionallyConnectionClientTest : TestBase
    {
        private string _deviceId;
        private string _apiKey;

        [TestInitialize]
        public void TestInit()
        {
            RegisterDevice();
        }

        [TestMethod]
        public void SingleDeviceRecordTelemetryDataTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey);

            ocassionalConnectionClient.RecordTelemetryData("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}");
        }

        [TestMethod]
        [ExpectedHttpStatusCode(HttpStatusCode.Unauthorized)]
        public void SingleDeviceRecordTelemetryDataAuthErrorTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey + "aaa");

            ocassionalConnectionClient.RecordTelemetryData("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}");
        }

        [TestMethod]
        public void SingleDeviceSendToTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey);

            ocassionalConnectionClient.SendMessageTo(_deviceId, "test");
        }

        [TestMethod]
        public void SingleDeviceSendToAndReceiveAndForgetTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey);

            ocassionalConnectionClient.SendMessageTo(_deviceId, "test");

            var result = ocassionalConnectionClient.ReceiveAndForgetMessage();

            Assert.IsTrue(result.MessageId >= 0);
            Assert.IsTrue(result.Payload == "test");
            Assert.IsTrue(result.Timestamp > new DateTime(2014,1,1));
            Assert.AreEqual(_deviceId, result.SenderDeviceId);
        }

        [TestMethod]
        public void SingleDeviceReceiveAndForgetNothingTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey);

            var result = ocassionalConnectionClient.ReceiveAndForgetMessage();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceSendToAndPeekAndCommitTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey);

            ocassionalConnectionClient.SendMessageTo(_deviceId, "msg1");
            ocassionalConnectionClient.SendMessageTo(_deviceId, "msg2");

            var result1 = ocassionalConnectionClient.PeekMessage();

            Assert.IsTrue(result1.MessageId >= 0);
            Assert.IsTrue(result1.Payload == "msg1");
            Assert.IsTrue(result1.Timestamp > new DateTime(2014, 1, 1));
            Assert.AreEqual(_deviceId, result1.SenderDeviceId);

            var result2 = ocassionalConnectionClient.PeekMessage();

            Assert.AreEqual(result1.MessageId, result2.MessageId);
            Assert.IsTrue(result2.Payload == "msg1");
            Assert.IsTrue(result2.Timestamp > new DateTime(2014, 1, 1));
            Assert.AreEqual(_deviceId, result2.SenderDeviceId);

            ocassionalConnectionClient.CommitMessage();

            var result3 = ocassionalConnectionClient.PeekMessage();

            Assert.AreEqual(result2.MessageId + 1, result3.MessageId);
            Assert.IsTrue(result3.Payload == "msg2");
            Assert.IsTrue(result3.Timestamp > new DateTime(2014, 1, 1));
        }

        [TestMethod]
        public void SingleDevicePeekNothingTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey);

            var result = ocassionalConnectionClient.PeekMessage();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleDeviceCommitNothingTest()
        {
            var ocassionalConnectionClient = new OccasionallyConnectionClient(PlatformApi, _deviceId, _apiKey);

            ocassionalConnectionClient.CommitMessage();
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

            _apiKey = managementClient.Service.Get(serviceId).ApiKey;
        }
    }
}
