using IoT.Framework;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IoT.Objects.Common.Tests
{
    [TestClass]
    public class NetworkAuthenticatorTest
    {
        [TestMethod]
        public void NetworkAuthOkTest()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var networkId = Identity.Next();
            var networkKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            networkOperations.Get(networkId).Returns(TestDataCreator.Network(networkId, networkKey, null, serviceId, null, null));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var networkAuthenticator = new NetworkAuthenticator(networkOperations, serviceOperations);

            Assert.IsTrue(networkAuthenticator.Authenticate(new AuthenticationParameters(networkId, networkKey)));
        }

        [TestMethod]
        public void ParentNetworkAuthOkTest()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var networkId = Identity.Next();
            var networkKey = Crypto.GenerateSafeRandomToken();

            var parentNetworkId = Identity.Next();
            var parentNetworkKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            networkOperations.Get(networkId).Returns(TestDataCreator.Network(networkId, networkKey, parentNetworkId, serviceId, null, null));
            networkOperations.Get(parentNetworkId).Returns(TestDataCreator.Network(parentNetworkId, parentNetworkKey, null, serviceId, null, null));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var networkAuthenticator = new NetworkAuthenticator(networkOperations, serviceOperations);

            Assert.IsTrue(networkAuthenticator.Authenticate(new AuthenticationParameters(networkId, parentNetworkKey)));
        }

        [TestMethod]
        public void ServiceAuthOkTest()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var networkId = Identity.Next();
            var networkKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            networkOperations.Get(networkId).Returns(TestDataCreator.Network(networkId, networkKey, null, serviceId, null, null));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var networkAuthenticator = new NetworkAuthenticator(networkOperations, serviceOperations);

            Assert.IsTrue(networkAuthenticator.Authenticate(new AuthenticationParameters(networkId, apiKey)));
        }

        [TestMethod]
        public void ServiceAuthFailTest()
        {
            var networkOperations = Substitute.For<INetworkOperations>();
            var serviceOperations = Substitute.For<IServiceOperations>();

            var networkId = Identity.Next();
            var networkKey = Crypto.GenerateSafeRandomToken();

            var serviceId = Identity.Next();
            var apiKey = Crypto.GenerateSafeRandomToken();

            networkOperations.Get(networkId).Returns(TestDataCreator.Network(networkId, networkKey, null, serviceId, null, null));
            serviceOperations.Get(serviceId).Returns(TestDataCreator.Service(serviceId, apiKey, null, null));

            var networkAuthenticator = new NetworkAuthenticator(networkOperations, serviceOperations);

            Assert.IsFalse(networkAuthenticator.Authenticate(new AuthenticationParameters(networkId, Identity.Next())));
        }
    }
}
