using System.Net.Mail;

namespace IoT.Framework.Mails
{
    public interface IMailSender
    {
        void Send(MailMessage mailMessage);
    }
}
