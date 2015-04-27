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
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingService);

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");

            var msg = messagingService.ReceiveAndForgetOutgoingMessage(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual(_deviceId, msg.Message.DeviceId);
            Assert.AreEqual("32412341243", msg.Message.Payload);

            var msg2 = messagingService.ReceiveAndForgetOutgoingMessage(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg2.State);
            Assert.IsNull(msg2.Message);

            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void RecordOutgoingMessageRealTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingService);

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");
            Assert.AreEqual(OutgoingState.Ok, success);

            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void PeekOutgoingMessageRealTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingService);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");

            var msg = messagingService.Peek(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual(_deviceId, msg.Message.DeviceId);
            Assert.AreEqual("32412341243", msg.Message.Payload);

            var msg2 = messagingService.Peek(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg2.State);
            Assert.AreEqual(_deviceId, msg2.Message.DeviceId);
            Assert.AreEqual("32412341243", msg2.Message.Payload);

            MessagingWorkers.Stop();
        }

        [TestMethod]
        public void CommitOutgoingMessageRealTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            MessagingWorkers.Start(new TestBatchParameters(), environmentFactory.MessagingService);

            var pltDeviceOperations = environmentFactory.ObjDeviceOperations;

            var messagingService = new MessagingService(new MessagingOperations(), pltDeviceOperations);

            messagingService.RecordOutgoingMessage(_deviceId, _deviceId, "32412341243");

            var msg = messagingService.Peek(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual(_deviceId, msg.Message.DeviceId);
            Assert.AreEqual("32412341243", msg.Message.Payload);

            var state = messagingService.Commit(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, state);

            var state2 = messagingService.Commit(_deviceId);
            Assert.AreEqual(OutgoingState.Ok, state2);

            MessagingWorkers.Stop();
        }
    }
}
