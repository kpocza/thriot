using Microsoft.Extensions.Configuration;
using Thriot.Framework.Mails;
using Thriot.Management.Services;

namespace Thriot.Management.WebApi.WebFunctions
{
    public class Mailer : IMailer
    {
        private readonly IConfiguration _configuration;

        public Mailer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendActivationMail(string userId, string displayName, string email, string activationCode, string websiteUrl)
        {
            var mail = new Mail(new MailSender(), new MailSettings(_configuration));

            mail.Send(Addressing.Create(email, displayName), "Activation", new
            {
                Name = displayName,
                Url = $"{websiteUrl}/User/Activate#{userId}/{activationCode}"
            });
        }

        public void SendForgotPasswordEmail(string userId, string displayName, string email, string confirmationCode, string websiteUrl)
        {
            var mail = new Mail(new MailSender(), new MailSettings(_configuration));

            mail.Send(Addressing.Create(email, displayName), "ResetPassword", new
            {
                Name = displayName,
                Url = $"{websiteUrl}/User/ResetPassword#{userId}/{confirmationCode}"
            });
        }
    }
}