using System;
using IoT.Platform.PersistentConnections.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IoT.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class CommandResolverTest
    {
        [TestMethod]
        public void ResolveLoginCommandTest()
        {
            var commandResolver = new CommandResolver("login deviceid apikey");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(LoginCommand));
            Assert.AreEqual("deviceid", ((LoginCommand)command).DeviceId);
            Assert.AreEqual("apikey", ((LoginCommand)command).ApiKey);
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void TryResolveLoginCommandTest()
        {
            var commandResolver = new CommandResolver("login deviceid");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(LoginCommand));
            Assert.IsFalse(command.IsValid);
        }

        [TestMethod]
        public void ResolveSubscribeCommandTest()
        {
            var commandResolver = new CommandResolver("subscribe receiveandforget");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(SubscribeCommand));
            Assert.AreEqual(SubscriptionType.ReceiveAndForget, ((SubscribeCommand)command).SubscriptionType);
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void ResolveUbsubscribeCommandTest()
        {
            var commandResolver = new CommandResolver("unsubscribe");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(UnsubscribeCommand));
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void ResolveCommitCommandTest()
        {
            var commandResolver = new CommandResolver("commit");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(CommitCommand));
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void ResolveHeartbeatCommandTest()
        {
            var commandResolver = new CommandResolver("heartbeat");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(HeartbeatCommand));
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void ResolveCloseCommandTest()
        {
            var commandResolver = new CommandResolver("close");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(CloseCommand));
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void ResolveMessageCommandTest()
        {
            long tickts = DateTime.UtcNow.Ticks;
            var commandResolver = new CommandResolver("telemetrydata {\"Temperature\": 24, \"Time\":" + tickts + "}");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(TelemetryDataCommand));
            Assert.AreEqual("{\"Temperature\":24,\"Time\":" + tickts + "}", ((TelemetryDataCommand)command).TelemetryData.ToString(Formatting.None));
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void ResolveMessageCommandNotValidJsonTest()
        {
            var commandResolver = new CommandResolver("telemetrydata {\"Temperature\": }");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(TelemetryDataCommand));
            Assert.IsFalse(command.IsValid);
        }

        [TestMethod]
        public void ResolveSendToCommandTest()
        {
            var commandResolver = new CommandResolver("sendto deviceId msg");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(SendToCommand));
            Assert.AreEqual("deviceId", ((SendToCommand)command).DeviceId);
            Assert.AreEqual("msg", ((SendToCommand)command).Message);
            Assert.IsTrue(command.IsValid);
        }

        [TestMethod]
        public void TryResolveSendToCommandTest()
        {
            var commandResolver = new CommandResolver("sendto deviceIdmsg");

            var command = commandResolver.GetCommand();

            Assert.IsInstanceOfType(command, typeof(SendToCommand));
            Assert.IsFalse(command.IsValid);
        }

        [TestMethod]
        public void TryResolveUnknownCommandTest()
        {
            var commandResolver = new CommandResolver("adfkghskdjfgh");

            var command = commandResolver.GetCommand();

            Assert.IsNull(command);
        }
    }
}
