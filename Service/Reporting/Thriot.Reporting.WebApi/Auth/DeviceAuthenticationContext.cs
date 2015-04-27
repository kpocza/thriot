using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;

namespace Thriot.Reporting.WebApi.Auth
{
    public class DeviceAuthenticationContext
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

            return claimsPrincipal.Identity.Name;
        }
    }
}