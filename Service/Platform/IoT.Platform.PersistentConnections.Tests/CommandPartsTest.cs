using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Platform.PersistentConnections.Tests
{
    [TestClass]
    public class CommandPartsTest
    {
        [TestMethod]
        public void SimpleCommandTest()
        {
            var commandParts = new CommandResolver.CommandParts("peek");

            Assert.AreEqual("peek", commandParts.Operation);
            Assert.IsNull(commandParts.Parameters);
        }

        [TestMethod]
        public void SimpleCommandCasingTest()
        {
            var commandParts = new CommandResolver.CommandParts("PeEk");

            Assert.AreEqual("PeEk", commandParts.Operation);
            Assert.IsNull(commandParts.Parameters);
        }

        [TestMethod]
        public void EmptyCommandTest()
        {
            var commandParts = new CommandResolver.CommandParts("");

            Assert.AreEqual("", commandParts.Operation);
            Assert.IsNull(commandParts.Parameters);
        }

        [TestMethod]
        public void CommandWithParamsTest()
        {
            var commandParts = new CommandResolver.CommandParts("login deviceid apikey");

            Assert.AreEqual("login", commandParts.Operation);
            Assert.AreEqual("deviceid apikey", commandParts.Parameters);
        }

        [TestMethod]
        public void CommandWithEmptyParamsTest()
        {
            var commandParts = new CommandResolver.CommandParts("login ");

            Assert.AreEqual("login", commandParts.Operation);
            Assert.AreEqual("", commandParts.Parameters);
        }
    }
}
