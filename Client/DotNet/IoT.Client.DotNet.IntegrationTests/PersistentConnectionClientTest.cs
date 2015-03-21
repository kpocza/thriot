using System;
using System.Collections.Generic;
using IoT.Client.DotNet.Management;
using IoT.Client.DotNet.Platform;
using IoT.Client.DotNet.Platform.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Client.DotNet.IntegrationTests
{
    [TestClass]
    public class PersistentConnectionClientTest : TestBase
    {
        private string _deviceId;
        private string _otherDeviceId;
        private string _apiKey;

        [TestMethod]
        public void LoginTest()
        {
            RegisterDevice();

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Login(_deviceId, _apiKey);
        }

        [TestMethod]
        [ExpectedException(typeof(LoginInvalidException))]
        public void TryInvalidCredentialsLoginTest()
        {
            RegisterDevice();

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Login(_deviceId, "123123");
        }

        [TestMethod]
        public void SubscribeUnsubscribeCloseTest()
        {
            RegisterDevice();

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Login(_deviceId, _apiKey);

            persistentConnection.Subscribe(SubscriptionType.ReceiveAndForget, message => { });

            persistentConnection.Unsubscribe();

            persistentConnection.Close();
        }

        [TestMethod]
        [ExpectedException(typeof(LoginRequiredException))]
        public void TrySubscribeNotLoggedInTest()
        {
            RegisterDevice();

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Subscribe(SubscriptionType.ReceiveAndForget, message => { });
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriptionRequiredException))]
        public void TryUnsubscribeTwiceTest()
        {
            RegisterDevice();

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Login(_deviceId, _apiKey);

            persistentConnection.Subscribe(SubscriptionType.ReceiveAndForget, message => { });

            persistentConnection.Unsubscribe();
            persistentConnection.Unsubscribe();
        }

        [TestMethod]
        public void TelemetryDataTest()
        {
            RegisterDevice();

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Login(_deviceId, _apiKey);

            persistentConnection.RecordTelemetryData("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}");

            persistentConnection.Close();
        }

        [TestMethod]
        [ExpectedException(typeof(MessageInvalidException))]
        public void TelemetryDataErrorNoSinkTest()
        {
            RegisterDevice(addMessageSinks: false);

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Login(_deviceId, _apiKey);

            persistentConnection.RecordTelemetryData("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}");
        }

        [TestMethod]
        public void SendToTest()
        {
            RegisterDevice(true);

            var persistentConnection = new PersistentConnectionClient(PlatformWebSocketApi);

            persistentConnection.Login(_deviceId, _apiKey);

            persistentConnection.SendMessageTo(_otherDeviceId, "{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}");

            persistentConnection.Close();
        }

        private void RegisterDevice(bool regOther = false, bool addMessageSinks = true)
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

            if (addMessageSinks)
            {
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
            }

            _deviceId = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "árvíztűrő tükörfúrógép" });

            _apiKey = managementClient.Service.Get(serviceId).ApiKey;

            if (regOther)
            {
                _otherDeviceId = managementClient.Device.Create(new Device { CompanyId = companyId, ServiceId = serviceId, NetworkId = networkId, Name = "árvíztűrő tükörfúrógép2" });
            }
        }
    }
}
