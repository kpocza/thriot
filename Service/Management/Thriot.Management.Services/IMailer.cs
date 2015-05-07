namespace Thriot.Management.Services
{
    public interface IMailer
    {
        void SendActivationMail(string userId, string displayName, string email, string activationCode, string managementApiUrl);

        void SendForgotPasswordEmail(string userId, string displayName, string email, string confirmationCode, string websiteUrl);
    }
}
