using System.IO;
using System.Net.Mail;

namespace Thriot.Framework.Mails
{
    public class MailSettings : IMailSettings
    {
        public MailAddress From
        {
            get { return new MailAddress("no-reply@thriot.cloudapp.net", "Thriot"); }
        }

        public string SmtpServer
        {
            get { return new SmtpClient().Host; }
        }

        public string BouncesAddress
        {
            get { return "bounces@thriot.cloudapp.net"; }
        }

        public Stream GetImageContent(string fileName)
        {
            return null;
        }
    }
}
