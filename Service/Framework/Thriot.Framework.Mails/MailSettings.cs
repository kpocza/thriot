using System.IO;
using System.Net.Mail;
using Microsoft.Framework.Configuration;

namespace Thriot.Framework.Mails
{
    public class MailSettings : IMailSettings
    {
        private readonly IConfiguration _configuration;

        public MailSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MailAddress From => new MailAddress(_configuration.Get("SmtpSettings:FromAddress"), _configuration.Get("SmtpSettings:FromName"));

        public string SmtpServer => _configuration.Get("SmtpSettings:Host");

        public int SmtpPort => int.Parse(_configuration.Get("SmtpSettings:Port"));

        public string Username => _configuration.Get("SmtpSettings:UserName");

        public string Password => _configuration.Get("SmtpSettings:Password");

        public string BouncesAddress => _configuration.Get("SmtpSettings:BouncesAddress");

        public Stream GetImageContent(string fileName)
        {
            return null;
        }
    }
}
