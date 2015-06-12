using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Thriot.Framework.Exceptions;
using Thriot.Objects.Model.Operations;
using Thriot.Platform.Model.Messaging;
using Thriot.TestHelpers;

namespace Thriot.Platform.Services.Messaging.Tests
{
    [TestClass]
    public class MessagingServiceTest
    {
        #region Record outgoing message

        [TestMethod]
        public void RecordOutgoingMessageTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Record(null).ReturnsForAnyArgs(OutgoingState.Ok);

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage("1234", "1234", "32412341243");

            Assert.AreEqual(OutgoingState.Ok, success);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void RecordOutgoingMessageDeviceNullTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();

            var messagingService = new MessagingService(messagingOperations, null);

            messagingService.RecordOutgoingMessage(null, null, "32412341243");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RecordOutgoingMessagePayloadNullTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();

            var messagingService = new MessagingService(messagingOperations, null);

            messagingService.RecordOutgoingMessage("1234", "1234124", null);
        }

        [TestMethod]
        public void RecordOutgoingMessagePayloadJustOkTest()
        {
            var messageRecorder = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messageRecorder.Record(null).ReturnsForAnyArgs(OutgoingState.Ok);

            var messagingService = new MessagingService(messageRecorder, pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage("1234", "1234", new string('a', 512));
            Assert.AreEqual(OutgoingState.Ok, success);
        }

        [TestMethod]
        public void RecordOutgoingMessagePayloadJustOkOtherDeviceTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            pltDeviceOperations.Get("1235").Returns(TestDataCreator.Device("1235", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Record(null).ReturnsForAnyArgs(OutgoingState.Ok);

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage("1234", "1235", new string('a', 512));
            Assert.AreEqual(OutgoingState.Ok, success);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void RecordOutgoingMessagePayloadJustOkOtherNetworkTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            pltDeviceOperations.Get("1235").Returns(TestDataCreator.Device("1235", "1234", "54321", "123456", "1234567", 1));
            messagingOperations.Record(null).ReturnsForAnyArgs(OutgoingState.Ok);

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var success = messagingService.RecordOutgoingMessage("1234", "1235", new string('a', 512));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RecordOutgoingMessagePayloadLong1Test()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();

            var messagingService = new MessagingService(messagingOperations, null);

            messagingService.RecordOutgoingMessage("1234", "1234124", new string('a', 513));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RecordOutgoingMessagePayloadLong2Test()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();

            var messagingService = new MessagingService(messagingOperations, null);

            messagingService.RecordOutgoingMessage("1234", "1234124", new string('a', 511) + 'á');
        }

        #endregion

        #region ReceiveAndForget outgoing message

        [TestMethod]

        public void ReceiveAndForgetOutgoingMessageTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.ReceiveAndForget(1)
                .Returns(new OutgoingMessageToStoreWithState(new OutgoingMessageToStore(1, new byte[] {48, 49, 50}, 1, DateTime.UtcNow, "sender"), OutgoingState.Ok));

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var msg = messagingService.ReceiveAndForgetOutgoingMessage("1234");

            Assert.IsNotNull(msg);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual("1234", msg.Message.DeviceId);
            Assert.AreEqual("012", msg.Message.Payload);
            Assert.AreEqual("sender", msg.Message.SenderDeviceId);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void ReceiveAndForgetOutgoingMessageNoDeviceTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            messagingService.ReceiveAndForgetOutgoingMessage(null);
        }

        [TestMethod]
        public void ReceiveAndForgetOutgoingMessageNoItemTset()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.ReceiveAndForget(1).Returns(new OutgoingMessageToStoreWithState(null, OutgoingState.Ok));

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var msg = messagingService.ReceiveAndForgetOutgoingMessage("1234");
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.IsNull(msg.Message);
        }

        [TestMethod]
        public void ReceiveAndForgetOutgoingMessageFailTset()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.ReceiveAndForget(1).Returns(new OutgoingMessageToStoreWithState(null, OutgoingState.Fail));

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var msg = messagingService.ReceiveAndForgetOutgoingMessage("1234");
            Assert.AreEqual(OutgoingState.Fail, msg.State);
            Assert.IsNull(msg.Message);
        }

        #endregion

        #region Peek outgoing message

        [TestMethod]

        public void PeekOutgoingMessageTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Peek(1)
                .Returns(new OutgoingMessageToStoreWithState(new OutgoingMessageToStore(1, new byte[] { 48, 49, 50 }, 1, DateTime.UtcNow, "sender"), OutgoingState.Ok));

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var msg = messagingService.Peek("1234");

            Assert.IsNotNull(msg);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual("1234", msg.Message.DeviceId);
            Assert.AreEqual("012", msg.Message.Payload);
            Assert.AreEqual("sender", msg.Message.SenderDeviceId);
        }

        [TestMethod]
        public void PeekTwiceOutgoingMessageTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Peek(1)
                .Returns(new OutgoingMessageToStoreWithState(new OutgoingMessageToStore(1, new byte[] { 48, 49, 50 }, 1, DateTime.UtcNow, "sender"), OutgoingState.Ok));

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var msg = messagingService.Peek("1234");

            Assert.IsNotNull(msg);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual("1234", msg.Message.DeviceId);
            Assert.AreEqual("012", msg.Message.Payload);
            Assert.AreEqual("sender", msg.Message.SenderDeviceId);

            msg = messagingService.Peek("1234");

            Assert.IsNotNull(msg);
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.AreEqual("1234", msg.Message.DeviceId);
            Assert.AreEqual("012", msg.Message.Payload);
            Assert.AreEqual("sender", msg.Message.SenderDeviceId);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void PeektOutgoingMessageNoDeviceTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            messagingService.Peek(null);
        }

        [TestMethod]
        public void PeekOutgoingMessageNoItemTset()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Peek(1).Returns(new OutgoingMessageToStoreWithState(null, OutgoingState.Ok));

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var msg = messagingService.Peek("1234");
            Assert.AreEqual(OutgoingState.Ok, msg.State);
            Assert.IsNull(msg.Message);
        }

        [TestMethod]
        public void PeekOutgoingMessageFailTset()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Peek(1).Returns(new OutgoingMessageToStoreWithState(null, OutgoingState.Fail));

            var messagingService = new MessagingService( messagingOperations, pltDeviceOperations);

            var msg = messagingService.Peek("1234");
            Assert.AreEqual(OutgoingState.Fail, msg.State);
            Assert.IsNull(msg.Message);
        }

        #endregion

        #region Commit outgoing message

        [TestMethod]
        public void CommitOutgoingMessageTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Commit(1)
                .Returns(OutgoingState.Ok);

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var state = messagingService.Commit("1234");

            Assert.AreEqual(OutgoingState.Ok, state);
        }

        [TestMethod]
        public void CommitTwiceOutgoingMessageTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Commit(1)
                .Returns(OutgoingState.Ok);

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var state = messagingService.Commit("1234");

            Assert.AreEqual(OutgoingState.Ok, state);

            state = messagingService.Commit("1234");

            Assert.AreEqual(OutgoingState.Ok, state);
        }

        [TestMethod]
        [ExpectedException(typeof(ForbiddenException))]
        public void CommitOutgoingMessageNoDeviceTest()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            messagingService.Commit(null);
        }

        [TestMethod]
        public void CommitOutgoingMessageFailTset()
        {
            var messagingOperations = Substitute.For<IMessagingOperations>();
            var pltDeviceOperations = Substitute.For<IDeviceOperations>();

            pltDeviceOperations.Get("1234").Returns(TestDataCreator.Device("1234", "1234", "12345", "123456", "1234567", 1));
            messagingOperations.Commit(1).Returns(OutgoingState.Fail);

            var messagingService = new MessagingService(messagingOperations, pltDeviceOperations);

            var state = messagingService.Commit("1234");
            Assert.AreEqual(OutgoingState.Fail, state);
        }

        #endregion
    }
}
