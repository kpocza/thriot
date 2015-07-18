using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace Thriot.Reporting.WebApi.Auth
{
    public class NetworkAuthenticationContext
    {
        public void RegisterContextNetwork(HttpContext context, string networkId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, networkId)
            };
            var id = new ClaimsIdentity(claims, "Network");
            var principal = new ClaimsPrincipal(new[] { id });

            context.User = principal;
        }

        public string GetContextNetwork(HttpContext context)
        {
            return context.User.Identity.Name;
        }
    }
}