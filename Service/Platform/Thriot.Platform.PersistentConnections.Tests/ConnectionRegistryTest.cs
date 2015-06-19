using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework;

namespace Thriot.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class ConnectionRegistryTest
    {
        [TestMethod]
        public void RegisterInitiatingConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            Assert.AreEqual(ConnectionState.Initiated, persistentConnection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, persistentConnection.SubscriptionType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TryRegisterInitiatingConnectionTwiceTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
        }

        [TestMethod]
        public void PromoteToLoggedInConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());
            
            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            Assert.IsNotNull(persistentConnection.DeviceId);
            Assert.AreEqual(ConnectionState.LoggedIn, persistentConnection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, persistentConnection.SubscriptionType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryPromoteToLoggedInConnectionNoDeviceIdTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TryPromoteToLoggedInConnectionTwiceTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            var deviceId = Identity.Next();

            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, deviceId, 1);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, deviceId, 1);
        }

        [TestMethod]
        public void PromoteToSubscribedConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);
            Assert.AreEqual(ConnectionState.LoggedIn | ConnectionState.Subscribed, persistentConnection.ConnectionState);
            Assert.AreEqual(SubscriptionType.PeekAndCommit, persistentConnection.SubscriptionType);
            persistentConnection.Received().Heartbeat();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TryPromoteToSubscribedConnectionTwiceTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);
            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TryPromoteToSubscribedConnectionNotLoggedInTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());
            persistentConnection.DeviceId.Returns(Identity.Next());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);
        }

        [TestMethod]
        public void UnsubscribeConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);
            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);

            connectionRegistry.UnsubscribeConnection(persistentConnection.DeviceId);

            Assert.AreEqual(ConnectionState.LoggedIn, persistentConnection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, persistentConnection.SubscriptionType);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TryUnsubscribeNonSubscribedConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.UnsubscribeConnection(persistentConnection.DeviceId);
        }

        [TestMethod]
        public void CloseInitiatingConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());
            persistentConnection.DeviceId.Returns((string)null);

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.CloseConnection(persistentConnection);
            Assert.AreEqual(ConnectionState.None, persistentConnection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, persistentConnection.SubscriptionType);
        }

        [TestMethod]
        public void CloseLoggedInConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.CloseConnection(persistentConnection);

            Assert.AreEqual(ConnectionState.None, persistentConnection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, persistentConnection.SubscriptionType);
        }

        [TestMethod]
        public void CloseSubscribedConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);
            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);

            connectionRegistry.CloseConnection(persistentConnection);

            Assert.AreEqual(ConnectionState.None, persistentConnection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, persistentConnection.SubscriptionType);
        }

        [TestMethod]
        public void StartStopWorkerTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());
            
            connectionRegistry.Start();
            connectionRegistry.Stop();
        }

        [TestMethod]
        public void CollectDeadInitiatedConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());
            persistentConnection.LastHeartbeat.Returns(new DateTime(2014, 1, 1));
            connectionRegistry.RegisterInitiatedConnection(persistentConnection);

            connectionRegistry.Start();

            int rounds = 0;
            while (persistentConnection.ConnectionState != ConnectionState.None && rounds < 100)
            {
                Thread.Sleep(10);
                rounds++;
            }

            connectionRegistry.Stop();
            persistentConnection.Received().Close();
            Assert.AreEqual(ConnectionState.None, persistentConnection.ConnectionState);
        }

        [TestMethod]
        public void CollectDeadLoggedinConnectionTest()
        {
            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), new DateTimeProvider());

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());
            persistentConnection.LastHeartbeat.Returns(new DateTime(2014, 1, 1));
            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, "1234", 1);

            connectionRegistry.Start();

            int rounds = 0;
            while (persistentConnection.ConnectionState != ConnectionState.None && rounds < 100)
            {
                Thread.Sleep(10);
                rounds++;
            }

            connectionRegistry.Stop();
            persistentConnection.Received().Close();
            Assert.AreEqual(ConnectionState.None, persistentConnection.ConnectionState);
        }
    }
}
