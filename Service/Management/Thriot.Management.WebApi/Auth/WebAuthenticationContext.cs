using Microsoft.AspNet.Hosting;
using System.Collections.Generic;
using System.Security.Claims;
using Thriot.Management.Services;

namespace Thriot.Management.WebApi.Auth
{
    public class WebAuthenticationContext : IAuthenticationContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebAuthenticationContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetContextUser(string userId)
        {
            var principal = BuildContextUserPrincipal(userId);
            var httpContext = _httpContextAccessor.HttpContext;
            
            httpContext.User = principal;
            httpContext.Authentication.SignIn(Microsoft.AspNet.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public void RemoveContextUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Authentication.SignOut();
        }

        public string GetContextUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var claimsPrincipal = httpContext.User;
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
