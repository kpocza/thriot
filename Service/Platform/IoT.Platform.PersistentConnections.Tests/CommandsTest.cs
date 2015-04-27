using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Thriot.Platform.PersistentConnections.Commands;

namespace Thriot.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class CommandsTest
    {
        [TestMethod]
        public void LoginCommandTest()
        {
            var loginCommand = new LoginCommand("deviceid apikey");

            Assert.IsTrue(loginCommand.IsValid);
            Assert.AreEqual("deviceid", loginCommand.DeviceId);
            Assert.AreEqual("apikey", loginCommand.ApiKey);
        }

        [TestMethod]
        public void LoginCommandFailTest()
        {
            var loginCommand = new LoginCommand("deviceid");

            Assert.IsFalse(loginCommand.IsValid);
        }

        [TestMethod]
        public void LoginCommandNullTest()
        {
            var loginCommand = new LoginCommand(null);

            Assert.IsFalse(loginCommand.IsValid);
        }

        [TestMethod]
        public void LoginCommandMoreParamsTest()
        {
            var loginCommand = new LoginCommand("a b c");

            Assert.IsFalse(loginCommand.IsValid);
        }

        [TestMethod]
        public void SubscribeCommandReceiveAndForgetTest()
        {
            var subscribeCommand = new SubscribeCommand("receiveandforget");

            Assert.IsTrue(subscribeCommand.IsValid);
            Assert.AreEqual(SubscriptionType.ReceiveAndForget, subscribeCommand.SubscriptionType);
        }

        [TestMethod]
        public void SubscribeCommandPeekAndCommitTest()
        {
            var subscribeCommand = new SubscribeCommand("peekandcommit");

            Assert.IsTrue(subscribeCommand.IsValid);
            Assert.AreEqual(SubscriptionType.PeekAndCommit, subscribeCommand.SubscriptionType);
        }

        [TestMethod]
        public void SubscribeCommandOtherTest()
        {
            var subscribeCommand = new SubscribeCommand("sdfgsdfg");

            Assert.IsFalse(subscribeCommand.IsValid);
        }

        [TestMethod]
        public void SubscribeCommandNullTest()
        {
            var subscribeCommand = new SubscribeCommand(null);

            Assert.IsFalse(subscribeCommand.IsValid);
        }

        [TestMethod]
        public void CommitCommandTest()
        {
            var commitCommand = new CommitCommand();

            Assert.IsTrue(commitCommand.IsValid);
        }

        [TestMethod]
        public void HeartbeatCommandTest()
        {
            var heartbeatCommand = new HeartbeatCommand();

            Assert.IsTrue(heartbeatCommand.IsValid);
        }

        [TestMethod]
        public void CloseCommandTest()
        {
            var closeCommand = new CloseCommand();

            Assert.IsTrue(closeCommand.IsValid);
        }

        [TestMethod]
        public void UnsubscribeCommandTest()
        {
            var unsubscribeCommand = new UnsubscribeCommand();

            Assert.IsTrue(unsubscribeCommand.IsValid);
        }

        [TestMethod]
        public void MessageCommandTest()
        {
            long ticks = DateTime.UtcNow.Ticks;
            var messageCommand = new TelemetryDataCommand("{\"Temperature\":24, \"Time\":" + ticks + "}");

            Assert.IsTrue(messageCommand.IsValid);
            Assert.AreEqual("{\"Temperature\":24,\"Time\":" + ticks + "}", messageCommand.TelemetryData.ToString(Formatting.None));
        }

        [TestMethod]
        public void MessageParseErrorTest()
        {
            var messageCommand = new TelemetryDataCommand("{\"Temperature\":}");

            Assert.IsFalse(messageCommand.IsValid);
        }

        [TestMethod]
        public void SendToCommandTest()
        {
            var sendToCommand = new SendToCommand("deviceId {\"Temperature\":24, \"Time\":" + DateTime.UtcNow.Ticks + "}");

            Assert.IsTrue(sendToCommand.IsValid);
            Assert.AreEqual("deviceId", sendToCommand.DeviceId);
            Assert.AreEqual("{\"Temperature\":24, \"Time\":" + DateTime.UtcNow.Ticks + "}", sendToCommand.Message);
        }

        [TestMethod]
        public void SendToCommandErrorTest()
        {
            var sendToCommand = new SendToCommand("deviceId{\"Temperature\":24}");

            Assert.IsFalse(sendToCommand.IsValid);
        }
    }
}
