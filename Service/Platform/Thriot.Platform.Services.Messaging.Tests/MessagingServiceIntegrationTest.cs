using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Platform.Model.Messaging;
using Thriot.TestHelpers;

namespace Thriot.Platform.Services.Messaging.Tests
{
    [TestClass]
    public class MessagingServiceIntegrationTest : TestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public void ReceiveAndForgetOutgoingMessageRealTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingServiceClient);

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");

            var msg = messagingService.ReceiveAndForgetOutgoingMessage(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual(_deviceId, msg.Message.DeviceId);
            Assert.AreEqual("32412341243", msg.Message.Payload);
            Assert.AreEqual(_deviceId, msg.Message.SenderDeviceId);

            var msg2 = messagingService.ReceiveAndForgetOutgoingMessage(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg2.State);
            Assert.IsNull(msg2.Message);

            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void RecordOutgoingMessageRealTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingServiceClient);

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");
            Assert.AreEqual(OutgoingState.Ok, success);

            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void PeekOutgoingMessageRealTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingServiceClient);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");

            var msg = messagingService.Peek(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual(_deviceId, msg.Message.DeviceId);
            Assert.AreEqual("32412341243", msg.Message.Payload);
            Assert.AreEqual(_deviceId, msg.Message.SenderDeviceId);

            var msg2 = messagingService.Peek(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg2.State);
            Assert.AreEqual(_deviceId, msg2.Message.DeviceId);
            Assert.AreEqual("32412341243", msg2.Message.Payload);
            Assert.AreEqual(_deviceId, msg.Message.SenderDeviceId);

            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void CommitOutgoingMessageRealTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingServiceClient);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");

            var msg = messagingService.Peek(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual(_deviceId, msg.Message.DeviceId);
            Assert.AreEqual("32412341243", msg.Message.Payload);
            Assert.AreEqual(_deviceId, msg.Message.SenderDeviceId);

            var state = messagingService.Commit(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, state);

            var state2 = messagingService.Commit(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, state2);

            MessagingWorkers.Stop();
        }
    }
}
