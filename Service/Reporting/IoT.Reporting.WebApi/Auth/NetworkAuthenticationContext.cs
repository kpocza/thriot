using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;

namespace IoT.Reporting.WebApi.Auth
{
    public class NetworkAuthenticationContext
    {
        public void RegisterContextNetwork(HttpRequestMessage request, string networkId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, networkId)
            };
            var id = new ClaimsIdentity(claims, "Network");
            var principal = new ClaimsPrincipal(new[] { id });

            request.GetOwinContext().Request.User = principal;
        }

        public string GetContextNetwork(HttpRequestMessage request)
        {
            var claimsPrincipal = (request.GetOwinContext().Request.User as ClaimsPrincipal);

            return claimsPrincipal.Identity.Name;
        }
    }
}