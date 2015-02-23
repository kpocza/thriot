using System;
using System.Security.Cryptography;
using System.Text;

namespace IoT.Framework
{
    public class Crypto
    {
        public static string GenerateSalt()
        {
            return GenerateSafeRandomToken();
        }

        public static string CalcualteHash(string password, string salt)
        {
            var sha256 = new SHA256Managed();

            var passwordHashBinary = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            string passwordHash = Convert.ToBase64String(passwordHashBinary);

            return passwordHash;
        }

        public static string GenerateSafeRandomToken()
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(bytes);

            return Convert.ToBase64String(bytes).Substring(0, 32);
        }
    }
}
