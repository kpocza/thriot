using Thriot.Framework.Mails;
using Thriot.Management.Services;

namespace Thriot.Management.WebApi.WebFunctions
{
    public class Mailer : IMailer
    {
        public void SendActivationMail(string userId, string displayName, string email, string activationCode, string managementApiUrl)
        {
            var mail = new Mail(new MailSender(), new MailSettings());

            mail.Send(Addressing.Create(email, displayName), "Activation", new
            {
                Name = displayName,
                Url = string.Format("{0}/users/activate/{1}/{2}", managementApiUrl, userId, activationCode)
            });
        }
    }
}