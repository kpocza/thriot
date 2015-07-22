using System.IO;
using System.Net.Mail;

namespace Thriot.Framework.Mails
{
    public interface IMailSettings
    {
        MailAddress From { get; }
        string SmtpServer { get; }

        int SmtpPort { get; }

        string Username { get; }

        string Password { get; }

        string BouncesAddress { get; }
        Stream GetImageContent(string fileName);
    }
}
