using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Thriot.Framework.Tests
{
    [TestClass]
    public class CryptoTest
    {
        [TestMethod]
        public void GenerateSalt1Test()
        {
            var salt = Crypto.GenerateSalt();

            Assert.AreEqual(32, salt.Length);
        }

        [TestMethod]
        public void GenerateSalt2Test()
        {
            var salt1 = Crypto.GenerateSalt();
            var salt2 = Crypto.GenerateSalt();

            Assert.AreNotEqual(salt1, salt2);
        }

        [TestMethod]
        public void Hash1Test()
        {
            var salt = Crypto.GenerateSalt();

            var passwordHash1 = Crypto.CalcualteHash("password1", salt);
            var passwordHash2 = Crypto.CalcualteHash("password2", salt);

            Assert.AreNotEqual(passwordHash1, passwordHash2);
        }

        [TestMethod]
        public void Hash2Test()
        {
            var salt1 = Crypto.GenerateSalt();
            var salt2 = Crypto.GenerateSalt();

            var passwordHash1 = Crypto.CalcualteHash("password", salt1);
            var passwordHash2 = Crypto.CalcualteHash("password", salt2);

            Assert.AreNotEqual(passwordHash1, passwordHash2);
        }
        [TestMethod]
        public void GenerateCryptoRandomToken1Test()
        {
            var token = Crypto.GenerateSafeRandomToken();

            Assert.AreEqual(32, token.Length);
        }

        [TestMethod]
        public void GenerateCryptoRandomToken2Test()
        {
            var token1 = Crypto.GenerateSafeRandomToken();
            var token2 = Crypto.GenerateSafeRandomToken();

            Assert.AreNotEqual(token1, token2);
        }
    }
}
