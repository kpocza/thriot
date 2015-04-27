using System;
using System.Net.Mail;
using Thriot.Framework.Logging;

namespace Thriot.Framework.Mails
{
    public class MailSender : IMailSender
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public void Send(MailMessage mailMessage)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.EnableSsl = true;
                    client.Timeout = 5000;
                    client.Send(mailMessage);
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
    }
}
