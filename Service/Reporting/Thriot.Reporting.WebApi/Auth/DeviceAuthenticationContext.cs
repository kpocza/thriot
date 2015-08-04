using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace Thriot.Reporting.WebApi.Auth
{
    public class DeviceAuthenticationContext
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

        public string GetContextDevice(HttpContext context)
        {
            return context.User.Identity.Name;
        }
    }
}