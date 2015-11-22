using System.IO;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Thriot.Framework.Mails
{
    public class MailSettings : IMailSettings
    {
        private readonly IConfiguration _configuration;

        public MailSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MailAddress From => new MailAddress(_configuration["SmtpSettings:FromAddress"], _configuration["SmtpSettings:FromName"]);

        public string SmtpServer => _configuration["SmtpSettings:Host"];

        public int SmtpPort => int.Parse(_configuration["SmtpSettings:Port"]);

        public string Username => _configuration["SmtpSettings:UserName"];

        public string Password => _configuration["SmtpSettings:Password"];

        public string BouncesAddress => _configuration["SmtpSettings:BouncesAddress"];

        public Stream GetImageContent(string fileName)
        {
            return null;
        }
    }
}
