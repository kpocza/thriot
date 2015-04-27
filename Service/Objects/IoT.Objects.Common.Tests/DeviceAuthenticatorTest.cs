using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;
using Thriot.Objects.Model;
using Thriot.Objects.Model.Operations;
using Thriot.TestHelpers;

namespace Thriot.Objects.Common.Tests
{
    [TestClass]
    public class DeviceAuthenticatorTest
    {
        [TestMethod]
        public void DeviceAuthOkTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var deviceId = Identity.Next();
            var deviceKey = Crypto.GenerateSafeRandomToken();

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, deviceKey, null, null, null, 0));

            var deviceAuthenticator = new DeviceAuthenticator(deviceOperations, networkOperations, serviceOperations);

            Assert.IsTrue(deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, deviceKey)));
        }

        [TestMethod]
        public void DeviceAuthNoSuchTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var deviceId = Identity.Next();
            var deviceKey = Crypto.GenerateSafeRandomToken();

            deviceOperations.Get(deviceId).Returns((Device)null);

            var deviceAuthenticator = new DeviceAuthenticator(deviceOperations, networkOperations, serviceOperations);

            Assert.IsFalse(deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, deviceKey)));
        }

        [TestMethod]
        public void NetworkAuthOkTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var deviceId = Identity.Next();
            var deviceKey = Crypto.GenerateSafeRandomToken();

            var networkId = Identity.Next();
            var networkKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, deviceKey, networkId, serviceId, null, 0));
            networkOperations.Get(networkId).Returns(TestDataCreator.Network(networkId, networkKey, null, serviceId, null, null));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var deviceAuthenticator = new DeviceAuthenticator(deviceOperations, networkOperations, serviceOperations);

            Assert.IsTrue(deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, networkKey)));
        }

        [TestMethod]
        public void ParentNetworkAuthOkTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var deviceId = Identity.Next();
            var deviceKey = Crypto.GenerateSafeRandomToken();

            var networkId = Identity.Next();
            var networkKey = Crypto.GenerateSafeRandomToken();

            var parentNetworkId = Identity.Next();
            var parentNetworkKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, deviceKey, networkId, serviceId, null, 0));
            networkOperations.Get(networkId).Returns(TestDataCreator.Network(networkId, networkKey, parentNetworkId, serviceId, null, null));
            networkOperations.Get(parentNetworkId).Returns(TestDataCreator.Network(parentNetworkId, parentNetworkKey, null, serviceId, null, null));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var deviceAuthenticator = new DeviceAuthenticator(deviceOperations, networkOperations, serviceOperations);

            Assert.IsTrue(deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, parentNetworkKey)));
        }

        [TestMethod]
        public void ServiceAuthOkTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var deviceId = Identity.Next();
            var deviceKey = Crypto.GenerateSafeRandomToken();

            var networkId = Identity.Next();
            var networkKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, deviceKey, networkId, serviceId, null, 0));
            networkOperations.Get(networkId).Returns(TestDataCreator.Network(networkId, networkKey, null, serviceId, null, null));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var deviceAuthenticator = new DeviceAuthenticator(deviceOperations, networkOperations, serviceOperations);

            Assert.IsTrue(deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, apiKey)));
        }

        [TestMethod]
        public void ServiceAuthFailTest()
        {
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var deviceId = Identity.Next();
            var deviceKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            deviceOperations.Get(deviceId).Returns(TestDataCreator.Device(deviceId, deviceKey, null, serviceId, null, 0));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var deviceAuthenticator = new DeviceAuthenticator(deviceOperations, networkOperations, serviceOperations);

            Assert.IsFalse(deviceAuthenticator.Authenticate(new AuthenticationParameters(deviceId, Identity.Next())));
        }
    }
}
