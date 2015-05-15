namespace Thriot.Client.DotNet.Management
{
    public class ResetPassword
    {
        public string UserId { get; set; }

        public string ConfirmationCode { get; set; }

        public string Password { get; set; }
    }
}
