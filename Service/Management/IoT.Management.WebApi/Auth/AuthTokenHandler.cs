using System;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using IoT.Management.Services;

namespace IoT.Management.WebApi.Auth
{
    public class AuthTokenHandler
    {
        private readonly IAuthenticationContext _authenticationContext;

        public AuthTokenHandler(IAuthenticationContext authenticationContext)
        {
            _authenticationContext = authenticationContext;
        }

        public string GenerateToken(string userId)
        {
            var ticket = GenerateTicket(userId);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes((ticket + ":0")));
        }

        public string ExtractToken(string authParams)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authParams));

            var parts = credentials.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                var authToken = parts[0];
                var zero = parts[1];

                if (zero == "0")
                {
                    return authToken;
                }
            }

            return null;
        }

        private string GenerateTicket(string userId)
        {
            var ticket = new FormsAuthenticationTicket(userId, false, 60 * 24);

            return FormsAuthentication.Encrypt(ticket);
        }

        public IPrincipal GenerateContextUser(string authToken)
        {
            var ticket = FormsAuthentication.Decrypt(authToken);

            var userId = ticket.Name;

            return _authenticationContext.GenerateContextUser(userId);
        }
    }
}
