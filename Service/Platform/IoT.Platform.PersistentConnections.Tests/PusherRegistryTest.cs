using System;
using IoT.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IoT.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class PusherRegistryTest
    {
        #region GetReceiveAndForgetConnectionTest

        [TestMethod]
        public void GetReceiveAndForgetConnectionEmptyTest()
        {
            var pusherRegistry = new PusherRegistry(new DateTimeProvider());

            var nothing = pusherRegistry.GetReceiveAndForgetConnection();
            Assert.IsNull(nothing);
        }

        [TestMethod]
        public void GetReceiveAndForgetConnectionNotYetSubscribedTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.ReceiveAndForget);
            connectionRegistry.UnsubscribeConnection(persistentConnection.DeviceId);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddMinutes(-10.1));

            var nothing = pusherRegistry.GetReceiveAndForgetConnection();
            Assert.IsNull(nothing);
        }

        [TestMethod]
        public void GetReceiveAndForgetConnectionHeartbeatMissedTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.ReceiveAndForget);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddSeconds(-41.0));

            var nothing = pusherRegistry.GetReceiveAndForgetConnection();
            Assert.IsNull(nothing);

            persistentConnection.ReceivedWithAnyArgs().NextReceiveAndForgetTime = new DateTime();
        }

        [TestMethod]
        public void GetReceiveAndForgetConnectionHeartbeatOkTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.ReceiveAndForget);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddSeconds(-20.0));
            persistentConnection.NextReceiveAndForgetTime.Returns(DateTime.UtcNow.AddSeconds(-0.1));

            var conn = pusherRegistry.GetReceiveAndForgetConnection();
            Assert.AreEqual(persistentConnection, conn);
            persistentConnection.ReceivedWithAnyArgs().NextReceiveAndForgetTime = new DateTime();
        }

        [TestMethod]
        public void GetReceiveAndForgetConnectionHeartbeatTooEarlyTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.ReceiveAndForget);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddSeconds(-20.0));
            persistentConnection.NextReceiveAndForgetTime.Returns(DateTime.UtcNow.AddSeconds(0.1));

            var nothing = pusherRegistry.GetReceiveAndForgetConnection();
            Assert.IsNull(nothing);
        }

	    #endregion    

        #region GetPeekConnectionTest

        [TestMethod]
        public void GetPeekConnectionEmptyTest()
        {
            var pusherRegistry = new PusherRegistry(new DateTimeProvider());

            var nothing = pusherRegistry.GetPeekConnection();
            Assert.IsNull(nothing);
        }

        [TestMethod]
        public void GetPeekConnectionNotYetSubscribedTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);
            connectionRegistry.UnsubscribeConnection(persistentConnection.DeviceId);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddMinutes(-10.1));

            var nothing = pusherRegistry.GetPeekConnection();
            Assert.IsNull(nothing);
        }

        [TestMethod]
        public void GetPeekConnectionHeartbeatMissedTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddSeconds(-41.0));

            var nothing = pusherRegistry.GetPeekConnection();
            Assert.IsNull(nothing);

            persistentConnection.ReceivedWithAnyArgs().NextPeekTime = new DateTime();
        }

        [TestMethod]
        public void GetPeekConnectionHeartbeatOkTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddSeconds(-20.0));
            persistentConnection.NextPeekTime.Returns(DateTime.UtcNow.AddSeconds(-0.1));

            var conn = pusherRegistry.GetPeekConnection();
            Assert.AreEqual(persistentConnection, conn);
            persistentConnection.ReceivedWithAnyArgs().NextPeekTime = new DateTime();
        }

        [TestMethod]
        public void GetPeekConnectionHeartbeatTooEarlyTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddSeconds(-20.0));
            persistentConnection.NextPeekTime.Returns(DateTime.UtcNow.AddSeconds(0.1));

            var nothing = pusherRegistry.GetPeekConnection();
            Assert.IsNull(nothing);
        }

        [TestMethod]
        public void GetPeekConnectionRequeueFromCommitToPeekTest()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var persistentConnection = Substitute.For<IPersistentConnection>();
            persistentConnection.ConnectionId.Returns(Guid.NewGuid());

            connectionRegistry.RegisterInitiatedConnection(persistentConnection);
            connectionRegistry.PromoteToLoggedInConnection(persistentConnection, Identity.Next(), 1);

            connectionRegistry.PromoteToSubscribedConnection(persistentConnection.DeviceId, SubscriptionType.PeekAndCommit);

            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow.AddSeconds(-20.0));
            persistentConnection.NextPeekTime.Returns(DateTime.UtcNow.AddSeconds(-0.1));

            var conn = pusherRegistry.GetPeekConnection();
            pusherRegistry.SetAsCommitNeededConnections(new[] {conn});

            persistentConnection.LastCommitTime = DateTime.UtcNow.AddSeconds(-20);

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow.AddMilliseconds(200));
            pusherRegistry.GetPeekConnection();
        }

        #endregion    
    }
}
