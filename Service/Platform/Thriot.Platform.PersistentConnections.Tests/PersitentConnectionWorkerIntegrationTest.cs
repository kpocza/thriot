using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Exceptions;
using Thriot.Framework;
using Thriot.Platform.Model;
using Thriot.Platform.Model.Messaging;
using Thriot.Platform.PersistentConnections.Commands;
using Thriot.Platform.Services.Messaging;
using Thriot.TestHelpers;

namespace Thriot.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class PersitentConnectionWorkerIntegrationTest : TestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void RecordOutgoingAndReceiveAndForgetMessageRealTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingService);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;
            var messagingOperations = new MessagingOperations();

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var batchParameters = Substitute.For<IBatchParameters>();
            batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionTime.Returns(
                TimeSpan.FromMilliseconds(100));
            batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionBatch.Returns(100);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);

            var persistentConnectionWorker = new PersistentConnectionReceiveAndForgetWorker(pusherRegistry, messagingOperations, batchParameters);
            persistentConnectionWorker.Start();

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");
            Assert.AreEqual(OutgoingState.Ok, success);

            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var connection = Substitute.For<IPersistentConnection>();

            connectionRegistry.RegisterInitiatedConnection(connection);
            connection.LastHeartbeat.Returns(DateTime.UtcNow);

            var device = _deviceService.Get(_deviceId);

            connectionRegistry.PromoteToLoggedInConnection(connection, _deviceId, device.NumericId);
            connectionRegistry.PromoteToSubscribedConnection(_deviceId, SubscriptionType.ReceiveAndForget);
            connection.NextReceiveAndForgetTime = DateTime.UtcNow.AddSeconds(-0.1);
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            bool ok = false;
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    connection.ReceivedWithAnyArgs().SendMessage(null);
                    ok = true;
                }
                catch (ReceivedCallsException)
                {
                    Thread.Sleep(100);
                }
            }

            Assert.IsTrue(ok);

            persistentConnectionWorker.Stop();
            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void RecordOutgoingAndPeekMessageRealTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingService);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;
            var messagingOperations = new MessagingOperations();

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var batchParameters = Substitute.For<IBatchParameters>();
            batchParameters.PersistentConnectionMessagePeekCollectionTime.Returns(
                TimeSpan.FromMilliseconds(100));
            batchParameters.PersistentConnectionMessagePeekCollectionBatch.Returns(100);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);

            var persistentConnectionWorker = new PersistentConnectionPeekWorker(pusherRegistry, messagingOperations, batchParameters);
            persistentConnectionWorker.Start();

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");
            Assert.AreEqual(OutgoingState.Ok, success);


            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var connection = Substitute.For<IPersistentConnection>();

            connectionRegistry.RegisterInitiatedConnection(connection);
            connection.LastHeartbeat.Returns(DateTime.UtcNow);

            var device = _deviceService.Get(_deviceId);

            connectionRegistry.PromoteToLoggedInConnection(connection, _deviceId, device.NumericId);
            connectionRegistry.PromoteToSubscribedConnection(_deviceId, SubscriptionType.PeekAndCommit);
            connection.NextPeekTime = DateTime.UtcNow.AddSeconds(-0.1);
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            bool ok = false;
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    connection.ReceivedWithAnyArgs().SendMessage(null);
                    ok = true;
                }
                catch (ReceivedCallsException)
                {
                    Thread.Sleep(100);
                }
            }

            Assert.IsTrue(ok);

            persistentConnectionWorker.Stop();
            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void RecordOutgoingAndPeekAndCommitMessageRealTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingService);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;
            var messagingOperations = new MessagingOperations();

            var batchParameters = Substitute.For<IBatchParameters>();
            batchParameters.PersistentConnectionMessagePeekCollectionTime.Returns(
                TimeSpan.FromMilliseconds(100));
            batchParameters.PersistentConnectionMessagePeekCollectionBatch.Returns(100);

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);

            var persistentConnectionWorker = new PersistentConnectionPeekWorker(pusherRegistry, messagingOperations, batchParameters);
            persistentConnectionWorker.Start();

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");
            Assert.AreEqual(OutgoingState.Ok, success);


            var connectionRegistry = new ConnectionRegistry(pusherRegistry, null);

            var connection = Substitute.For<IPersistentConnection>();

            connectionRegistry.RegisterInitiatedConnection(connection);
            connection.LastHeartbeat.Returns(DateTime.UtcNow);

            var device = _deviceService.Get(_deviceId);

            connectionRegistry.PromoteToLoggedInConnection(connection, _deviceId, device.NumericId);
            connectionRegistry.PromoteToSubscribedConnection(_deviceId, SubscriptionType.PeekAndCommit);
            connection.NextPeekTime = DateTime.UtcNow.AddSeconds(-0.1);
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            bool ok = false;
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    connection.ReceivedWithAnyArgs().SendMessage(null);
                    ok = true;
                }
                catch (ReceivedCallsException)
                {
                    Thread.Sleep(100);
                }
            }

            Assert.IsTrue(ok);

            var commandExecutor = new CommandExecutor(pusherRegistry, connectionRegistry, messagingOperations, null, null, null, null);
            commandExecutor.Execute(connection, new CommitCommand());


            ok = false;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    connection.ReceivedWithAnyArgs().SendMessage(null);
                    ok = true;
                }
                catch (ReceivedCallsException)
                {
                    Thread.Sleep(100);
                }
            }

            Assert.IsTrue(ok);

            persistentConnectionWorker.Stop();
            MessagingWorkers.Stop();
        }
    }
}
