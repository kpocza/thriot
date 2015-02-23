using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace IoT.Framework
{
    public static class Validator
    {
        private static readonly Regex IdRegex = new Regex("^[0-9A-Fa-f]{32}$", RegexOptions.Compiled);

        public static string TrimAndValidateAsName(string name, int maxLength = 50)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            name = name.Trim();

            if(name.Length > maxLength)
                throw new ArgumentOutOfRangeException("name");

            return name;
        }

        public static void ValidateId(string id)
        {
            if(id == null)
                throw new ArgumentNullException("id");

            if (!IdRegex.IsMatch(id))
                throw new ArgumentOutOfRangeException("id");
        }

        public static string ValidateEmail(string email, int maxLength = 128)
        {
            if(string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("email");

            email = email.Trim();

            if (email.Length > maxLength)
                throw new ArgumentOutOfRangeException("email");

            try
            {
                new MailAddress(email);
                return email;
            }
            catch
            {
                throw new ArgumentOutOfRangeException("email");
            }
        }

        public static void ValidatePassword(string password)
        {
            if(string.IsNullOrWhiteSpace(password) || password.Length < 5)
                throw new ArgumentOutOfRangeException("password");
        }
    }
}
