using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;

namespace IoT.Platform.WebApi.Auth
{
    public class AuthenticationContext
    {
        public void RegisterContextDevice(HttpRequestMessage request, string deviceId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, deviceId)
            };
            var id = new ClaimsIdentity(claims, "Device");
            var principal = new ClaimsPrincipal(new[] { id });
            
            request.GetOwinContext().Request.User = principal;
        }

        public string GetContextDevice(HttpRequestMessage request)
        {
            var claimsPrincipal = (request.GetOwinContext().Request.User as ClaimsPrincipal);

            if (claimsPrincipal == null || claimsPrincipal.Identity == null)
                return null;

            return claimsPrincipal.Identity.Name;
        }
    }
}