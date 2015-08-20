using System;
using System.Net;
using System.Net.Mail;
using Thriot.Framework.Logging;

namespace Thriot.Framework.Mails
{
    public class MailSender : IMailSender
    {
        private static readonly ILogger Logger = LoggerFactory.GetCurrentClassLogger();

        public void Send(MailMessage mailMessage, IMailSettings mailSettings)
        {
            try
            {
                using (var client = new SmtpClient(mailSettings.SmtpServer, mailSettings.SmtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(mailSettings.Username, mailSettings.Password);
                    client.EnableSsl = true;
                    client.Timeout = 10000;
                    client.Send(mailMessage);
                }
            }
            catch(Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}
