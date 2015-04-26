using IoT.Framework;
using IoT.Framework.Exceptions;
using IoT.Management.Model;
using IoT.UnitTestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.Tests
{
    [TestClass]
    public class UserOperationsTest
    {
        [TestMethod]
        public void CreateTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var id = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        public void IsNotExistsTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;

            Assert.IsFalse(userOperations.IsExists("nosuch"));
        }

        [TestMethod]
        public void IsExistsTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;

            var email = EmailHelper.Generate();

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            userOperations.Create(new User() { Name = "new user", Email = email }, passwordHash, salt);

            Assert.IsTrue(userOperations.IsExists(email));
        }

        [TestMethod]
        public void GetMeSuccessTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;

            var email = EmailHelper.Generate();

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var id = userOperations.Create(new User() { Name = "new user", Email = email, Activated = true, ActivationCode = "12345" }, passwordHash, salt);

            var user = userOperations.Get(id);

            Assert.AreEqual(id, user.Id);
            Assert.AreEqual("new user", user.Name);
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(true, user.Activated);
            Assert.AreEqual("12345", user.ActivationCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void GetMeFailedTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;

            userOperations.Get("231413245");
        }

        [TestMethod]
        public void UpdateSuccessTest()
        {
            var environmentFactory = SingleContainer.Instance.Resolve<IEnvironmentFactory>();
            var userOperations = environmentFactory.MgmtUserOperations;

            var email = EmailHelper.Generate();

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var id = userOperations.Create(new User() { Name = "new user", Email = email, Activated = true, ActivationCode = "12345" }, passwordHash, salt);

            var user = userOperations.Get(id);

            user.Activated = false;
            user.ActivationCode = "54321";

            userOperations.Update(user);

            user = userOperations.Get(id);

            Assert.AreEqual(id, user.Id);
            Assert.AreEqual("new user", user.Name);
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(false, user.Activated);
            Assert.AreEqual("54321", user.ActivationCode);
        }
    }
}
