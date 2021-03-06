﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thriot.Framework;
using Thriot.Framework.Exceptions;
using Thriot.Management.Model;
using Thriot.TestHelpers;

namespace Thriot.Management.Operations.Tests
{
    [TestClass]
    public class UserOperationsTest
    {
        [TestMethod]
        public void CreateTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var id = userOperations.Create(new User() { Name = "new user", Email = EmailHelper.Generate() }, passwordHash, salt);

            Assert.AreEqual(32, id.Length);
        }

        [TestMethod]
        public void IsNotExistsTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;

            Assert.IsFalse(userOperations.IsExists("nosuch"));
        }

        [TestMethod]
        public void IsExistsTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;

            var email = EmailHelper.Generate();

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            userOperations.Create(new User() { Name = "new user", Email = email }, passwordHash, salt);

            Assert.IsTrue(userOperations.IsExists(email));
        }

        [TestMethod]
        public void GetMeSuccessTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;

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
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;

            userOperations.Get("231413245");
        }

        [TestMethod]
        public void UpdateSuccessTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;

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


        [TestMethod]
        public void UpdateLoginUserTest()
        {
            var environmentFactory = EnvironmentFactoryFactory.Create();
            var userOperations = environmentFactory.ManagementEnvironment.MgmtUserOperations;

            var email = EmailHelper.Generate();

            var salt = Crypto.GenerateSalt();
            var passwordHash = Crypto.CalcualteHash("password", salt);
            var id = userOperations.Create(new User() { Name = "new user", Email = email, Activated = true, ActivationCode = "12345" }, passwordHash, salt);

            var loginUser = userOperations.GetLoginUser(email);

            var salt2 = Crypto.GenerateSalt();
            var passwordHash2 = Crypto.CalcualteHash("password2", salt);

            loginUser.PasswordHash = passwordHash2;
            loginUser.Salt = salt2;

            userOperations.Update(loginUser);

            var loginUser2 = userOperations.GetLoginUser(email);

            Assert.AreEqual(id, loginUser2.UserId);
            Assert.AreEqual(email, loginUser2.Email);
            Assert.AreEqual(passwordHash2, loginUser2.PasswordHash);
            Assert.AreEqual(salt2, loginUser2.Salt);
        }
    }
}
