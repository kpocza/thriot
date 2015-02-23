using System;
using System.Collections.Generic;
using IoT.Framework;
using IoT.Objects.Model;
using IoT.Objects.Model.Operations;
using IoT.Platform.Model;
using IoT.Platform.Model.Messaging;
using IoT.Platform.PersistentConnections.Commands;
using IoT.Platform.Services.Messaging;
using IoT.Platform.Services.Telemetry;
using IoT.Plugins.Core;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IoT.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class CommandExecutorTest
    {
        [TestMethod]
        public void HandleBadCommand1Test()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var commandExecutor = new CommandExecutor(null, null, null, null, null, null, null);
            
            commandExecutor.Execute(connection, null);

            connection.Received().Reply("badcommand");
        }

        [TestMethod]
        public void HandleBadCommand2Test()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var commandExecutor = new CommandExecutor(null, null, null, null, null, null, null);

            commandExecutor.Execute(connection, new SubscribeCommand("asdfadsgsdfgsdg"));

            connection.Received().Reply("badcommand");
        }

        [TestMethod]
        public void LoginTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));
            
            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            connection.Received().Reply("login ack");
            Assert.AreEqual(ConnectionState.LoggedIn, connection.ConnectionState);
            Assert.AreEqual(1, connection.NumericDeviceId);
        }

        [TestMethod]
        public void TryLoginNotInitiatedTest()
        {
            var connection = Substitute.For<IPersistentConnection>();

            connection.ConnectionId.Returns(Guid.NewGuid());

            var commandExecutor = new CommandExecutor(null, null, null, null, null, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            connection.Received().Reply("login badcommand");
            Assert.AreEqual(ConnectionState.None, connection.ConnectionState);
        }

        [TestMethod]
        public void TryLoginUnauthorizedTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(false);

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, null, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            connection.Received().Reply("login unauthorized");
            Assert.AreEqual(ConnectionState.Initiated, connection.ConnectionState);
        }

        [TestMethod]
        public void TryLoginSameDeviceTwiceTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var connection2 = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            connection2.ConnectionId.Returns(Guid.NewGuid());

            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            var deviceId = Identity.Next();

            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);
            connectionRegistry.RegisterInitiatedConnection(connection2);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(deviceId + " " + Identity.Next()));
            commandExecutor.Execute(connection2, new LoginCommand(deviceId + " " + Identity.Next()));

            connection.Received().Reply("login ack");
            connection2.Received().Reply("login badcommand");
            Assert.AreEqual(ConnectionState.LoggedIn, connection.ConnectionState);
            Assert.AreEqual(1, connection.NumericDeviceId);
        }

        [TestMethod]
        public void SubscribeTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            commandExecutor.Execute(connection, new SubscribeCommand("receiveandforget"));

            connection.Received().Heartbeat();
            connection.Received().Reply("subscribe ack");
            Assert.AreEqual(ConnectionState.LoggedIn | ConnectionState.Subscribed, connection.ConnectionState);
            Assert.AreEqual(SubscriptionType.ReceiveAndForget, connection.SubscriptionType);
        }

        [TestMethod]
        public void TrySubscribeWithoutLoginTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new SubscribeCommand("receiveandforget"));

            connection.Received().Reply("subscribe unauthorized");
            Assert.AreEqual(ConnectionState.Initiated, connection.ConnectionState);
        }

        [TestMethod]
        public void TrySubscribeTwiceTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            commandExecutor.Execute(connection, new SubscribeCommand("receiveandforget"));

            commandExecutor.Execute(connection, new SubscribeCommand("receiveandforget"));

            connection.Received().Reply("subscribe already");
            Assert.AreEqual(ConnectionState.LoggedIn | ConnectionState.Subscribed, connection.ConnectionState);
            Assert.AreEqual(SubscriptionType.ReceiveAndForget, connection.SubscriptionType);
        }

        [TestMethod]
        public void UnsubscribeTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            commandExecutor.Execute(connection, new SubscribeCommand("receiveandforget"));

            commandExecutor.Execute(connection, new UnsubscribeCommand());

            connection.Received().Reply("unsubscribe ack");
            Assert.AreEqual(ConnectionState.LoggedIn, connection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, connection.SubscriptionType);
        }

        [TestMethod]
        public void TryUnSubscribeNotLoggedinTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new UnsubscribeCommand());

            connection.Received().Reply("unsubscribe unauthorized");
            Assert.AreEqual(ConnectionState.Initiated, connection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, connection.SubscriptionType);
        }

        [TestMethod]
        public void TryUnSubscribeNotsubscribedTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            commandExecutor.Execute(connection, new UnsubscribeCommand());

            connection.Received().Reply("unsubscribe notsubscribed");
            Assert.AreEqual(ConnectionState.LoggedIn, connection.ConnectionState);
            Assert.AreEqual(SubscriptionType.None, connection.SubscriptionType);
        }

        [TestMethod]
        public void CloseTest()
        {
            var connection = Substitute.For<IPersistentConnection>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, null, null, null, null);

            commandExecutor.Execute(connection, new CloseCommand());

            Assert.AreEqual(ConnectionState.None, connection.ConnectionState);
            connection.Received().Close();
        }

        [TestMethod]
        public void HeartbeatTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();

            var connectionRegistry = new ConnectionRegistry(new PusherRegistry(new DateTimeProvider()), null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(null, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            commandExecutor.Execute(connection, new SubscribeCommand("receiveandforget"));

            commandExecutor.Execute(connection, new HeartbeatCommand());

            Assert.AreEqual(ConnectionState.LoggedIn | ConnectionState.Subscribed, connection.ConnectionState);
            connection.Received().Heartbeat();
        }

        [TestMethod]
        public void CommitSuccessTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var outgoingMessageReader = Substitute.For<IMessagingOperations>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, outgoingMessageReader, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(deviceId + " " + Identity.Next()));

            commandExecutor.Execute(connection, new SubscribeCommand("peekandcommit"));

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            pusherRegistry.SetAsCommitNeededConnections(new[] { connection });

            outgoingMessageReader.Commit((long)0).ReturnsForAnyArgs(OutgoingState.Ok);

            Assert.IsTrue(pusherRegistry.IsCommitable(deviceId));

            commandExecutor.Execute(connection, new CommitCommand());

            Assert.IsFalse(pusherRegistry.IsCommitable(deviceId));

            commandExecutor.Execute(connection, new CommitCommand());
        }

        [TestMethod]
        public void CommitNotSubscribedTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var outgoingMessageReader = Substitute.For<IMessagingOperations>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, outgoingMessageReader, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(deviceId + " " + Identity.Next()));

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            pusherRegistry.SetAsCommitNeededConnections(new[] { connection });

            outgoingMessageReader.Commit((long)0).ReturnsForAnyArgs(OutgoingState.Ok);

            Assert.IsTrue(pusherRegistry.IsCommitable(deviceId));
            
            connection.ClearReceivedCalls();

            commandExecutor.Execute(connection, new CommitCommand());
            connection.DidNotReceive().Heartbeat();
        }

        [TestMethod]
        public void CommitNotLoggedInTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var outgoingMessageReader = Substitute.For<IMessagingOperations>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, outgoingMessageReader, deviceAuthenticator, deviceOperations, null, null);

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            pusherRegistry.SetAsCommitNeededConnections(new[] { connection });

            outgoingMessageReader.Commit((long)0).ReturnsForAnyArgs(OutgoingState.Ok);

            connection.ClearReceivedCalls();

            commandExecutor.Execute(connection, new CommitCommand());
            connection.DidNotReceive().Heartbeat();
        }

        [TestMethod]
        public void CommitNotApplicableTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();

            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device("1234", "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(Identity.Next() + " " + Identity.Next()));

            commandExecutor.Execute(connection, new SubscribeCommand("peekandcommit"));

            commandExecutor.Execute(connection, new CommitCommand());
        }

        [TestMethod]
        public void TelemetryDataSuccessTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var inc = Substitute.For<ITelemetryDataSink>();

            telemetryDataSinkResolver.ResolveIncoming(null).ReturnsForAnyArgs(new List<ITelemetryDataSink>(){inc});
            var telemetryDataService = new TelemetryDataService(telemetryDataSinkResolver);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, telemetryDataService);

            commandExecutor.Execute(connection, new LoginCommand(deviceId + " " + Identity.Next()));

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            commandExecutor.Execute(connection, new TelemetryDataCommand("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
            connection.Received(1).Reply("telemetrydata ack");
        }

        [TestMethod]
        public void TelemetryDataErrorTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            telemetryDataSinkResolver.ResolveIncoming(null).ReturnsForAnyArgs(new List<ITelemetryDataSink>() { });
            var telemetryDataService = new TelemetryDataService(telemetryDataSinkResolver);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, telemetryDataService);

            commandExecutor.Execute(connection, new LoginCommand(deviceId + " " + Identity.Next()));

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            commandExecutor.Execute(connection, new TelemetryDataCommand("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
            connection.Received(1).Reply("telemetrydata error");
        }

        [TestMethod]
        public void TelemetryDataLoginRequiredTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var telemetryDataSinkResolver = Substitute.For<ITelemetryDataSinkResolver>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            telemetryDataSinkResolver.ResolveIncoming(null).ReturnsForAnyArgs(new List<ITelemetryDataSink>());
            var telemetryDataService = new TelemetryDataService(telemetryDataSinkResolver);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, telemetryDataService);

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            commandExecutor.Execute(connection, new TelemetryDataCommand("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
            connection.Received(1).Reply("telemetrydata unauthorized");
        }

        [TestMethod]
        public void SendToSuccessTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var messagingOperations = Substitute.For<IMessagingOperations>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var msgService = new MessagingService(messagingOperations, deviceOperations);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, null, deviceAuthenticator, deviceOperations, msgService, null);

            commandExecutor.Execute(connection, new LoginCommand(deviceId + " " + Identity.Next()));

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            commandExecutor.Execute(connection, new SendToCommand("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
            connection.Received(1).Reply("sendto ack");
        }

        [TestMethod]
        public void SendToLoginRequiredTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            commandExecutor.Execute(connection, new SendToCommand("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
            connection.Received(1).Reply("sendto unauthorized");
        }

        [TestMethod]
        public void SendToErrorTest()
        {
            var connection = Substitute.For<IPersistentConnection>();
            var deviceAuthenticator = Substitute.For<IDeviceAuthenticator>();
            var deviceOperations = Substitute.For<IDeviceOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
            var pusherRegistry = new PusherRegistry(dateTimeProvider);
            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var deviceId = Identity.Next();

            connection.ConnectionId.Returns(Guid.NewGuid());
            deviceAuthenticator.Authenticate(null).ReturnsForAnyArgs(true);
            deviceOperations.Get(null).ReturnsForAnyArgs(TestDataCreator.Device(deviceId, "1234", "2345", "3456", "4567", 1));

            connectionRegistry.RegisterInitiatedConnection(connection);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, null, deviceAuthenticator, deviceOperations, null, null);

            commandExecutor.Execute(connection, new LoginCommand(deviceId + " " + Identity.Next()));

            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            commandExecutor.Execute(connection, new SendToCommand("{\"Temperature\": 24, \"Time\":" + DateTime.UtcNow.Ticks + "}"));
            connection.Received(1).Reply("sendto error");
        }
    }
}
