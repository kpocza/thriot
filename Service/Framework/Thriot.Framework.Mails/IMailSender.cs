using System.Net.Mail;

namespace Thriot.Framework.Mails
{
    public interface IMailSender
    {
        void Send(MailMessage mailMessage, IMailSettings mailSettings);
    }
}
