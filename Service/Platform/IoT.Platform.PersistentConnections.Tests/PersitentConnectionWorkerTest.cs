using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Exceptions;
using Thriot.Framework;
using Thriot.Platform.Model;
using Thriot.Platform.Model.Messaging;

namespace Thriot.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class PersitentConnectionWorkerTest
    {

        [TestMethod]
        public void StartStopTest()
        {
            var outgoingMessageReader = Substitute.For<IMessagingOperations>();
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var batchParameters = Substitute.For<IBatchParameters>();
            batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionTime.Returns(
                TimeSpan.FromMilliseconds(100));
            batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionBatch.Returns(100);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);

            var persistentConnectionWorker = new PersistentConnectionReceiveAndForgetWorker(pusherRegistry, outgoingMessageReader, batchParameters);

            persistentConnectionWorker.Start();
            persistentConnectionWorker.Stop();
        }

        [TestMethod]
        public void StartStopWithReceiveAndForgetProcessingElementTest()
        {
            var outgoingMessageReader = Substitute.For<IMessagingOperations>();
            var persistentConnection = Substitute.For<IPersistentConnection>();

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var batchParameters = Substitute.For<IBatchParameters>();
            batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionTime.Returns(
                TimeSpan.FromMilliseconds(100));
            batchParameters.PersistentConnectionMessageReceiveAndForgetCollectionBatch.Returns(100);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);

            persistentConnection.DeviceId = "1234";
            persistentConnection.NumericDeviceId = 1;
            persistentConnection.ConnectionState = ConnectionState.LoggedIn | ConnectionState.Subscribed;
            persistentConnection.SubscriptionType = SubscriptionType.ReceiveAndForget;
            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow);
            outgoingMessageReader.ReceiveAndForgetMany(null)
                .ReturnsForAnyArgs(new Dictionary<long, OutgoingMessageToStoreWithState>()
                {
                    {
                        1,
                        new OutgoingMessageToStoreWithState(new OutgoingMessageToStore(1, Encoding.UTF8.GetBytes("{}"), 1,
                            DateTime.UtcNow), OutgoingState.Ok)
                    }
                });

            pusherRegistry.AddConnection(persistentConnection);

            persistentConnection.NextReceiveAndForgetTime = DateTime.UtcNow.AddSeconds(-2);

            var persistentConnectionWorker = new PersistentConnectionReceiveAndForgetWorker(pusherRegistry, outgoingMessageReader, batchParameters);

            persistentConnectionWorker.Start();
            bool ok = false;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    persistentConnection.ReceivedWithAnyArgs().SendMessage(null);
                    ok = true;
                }
                catch (ReceivedCallsException)
                {
                    Thread.Sleep(100);
                }
            }

            Assert.IsTrue(ok);

            persistentConnectionWorker.Stop();
        }


        [TestMethod]
        public void StartStopWithPeekProcessingElementTest()
        {
            var outgoingMessageReader = Substitute.For<IMessagingOperations>();
            var persistentConnection = Substitute.For<IPersistentConnection>();

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

            var batchParameters = Substitute.For<IBatchParameters>();
            batchParameters.PersistentConnectionMessagePeekCollectionTime.Returns(
                TimeSpan.FromMilliseconds(100));
            batchParameters.PersistentConnectionMessagePeekCollectionBatch.Returns(100);

            var pusherRegistry = new PusherRegistry(dateTimeProvider);

            persistentConnection.DeviceId = "1234";
            persistentConnection.NumericDeviceId = 1;
            persistentConnection.ConnectionState = ConnectionState.LoggedIn | ConnectionState.Subscribed;
            persistentConnection.SubscriptionType = SubscriptionType.PeekAndCommit;
            persistentConnection.LastHeartbeat.Returns(DateTime.UtcNow);
            outgoingMessageReader.PeekMany(null)
                .ReturnsForAnyArgs(new Dictionary<long, OutgoingMessageToStoreWithState>()
                {
                    {
                        1,
                        new OutgoingMessageToStoreWithState(new OutgoingMessageToStore(1, Encoding.UTF8.GetBytes("{}"), 1,
                            DateTime.UtcNow), OutgoingState.Ok)
                    }
                });

            pusherRegistry.AddConnection(persistentConnection);

            persistentConnection.NextPeekTime = DateTime.UtcNow.AddSeconds(-2);

            var persistentConnectionWorker = new PersistentConnectionPeekWorker(pusherRegistry, outgoingMessageReader, batchParameters);

            persistentConnectionWorker.Start();
            bool ok = false;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    persistentConnection.ReceivedWithAnyArgs().SendMessage(null);
                    ok = true;
                }
                catch (ReceivedCallsException)
                {
                    Thread.Sleep(100);
                }
            }

            Assert.IsTrue(ok);

            persistentConnectionWorker.Stop();
        }

    }
}
