using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace Thriot.Platform.WebApi.Auth
{
    public class AuthenticationContext
    {
        public void RegisterContextDevice(HttpContext context, string deviceId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, deviceId)
            };
            var id = new ClaimsIdentity(claims, "Device");
            var principal = new ClaimsPrincipal(new[] { id });
            
            context.User = principal;
        }

        public string GetContextDevice(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null || claimsPrincipal.Identity == null)
                return null;

            return claimsPrincipal.Identity.Name;
        }
    }
}