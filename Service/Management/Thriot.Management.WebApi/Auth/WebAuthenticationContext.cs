using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Thriot.Management.Services;

namespace Thriot.Management.WebApi.Auth
{
    public class WebAuthenticationContext : IAuthenticationContext
    {
        private HttpContext _httpContext;

        public WebAuthenticationContext(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void SetContextUser(string userId)
        {
            var principal = BuildContextUserPrincipal(userId);
            _httpContext.User = principal;
            _httpContext.Response.SignIn("Cookie", principal);
        }

        public void RemoveContextUser()
        {
            _httpContext.Response.SignOut();
        }

        public string GetContextUser()
        {
            var claimsPrincipal = _httpContext.User;
            if (claimsPrincipal == null || claimsPrincipal.Identities == null)
                return null;

            var name = claimsPrincipal.Identity.Name;

            if (name == "")
                name = null;

            return name;
        }

        private ClaimsPrincipal BuildContextUserPrincipal(string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userId)
            };

            var id = new ClaimsIdentity(claims, "ApplicationCookie");
            return new ClaimsPrincipal(new[] { id });
        }
    }
}
