using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Thriot.Management.Services;

namespace Thriot.Management.WebApi.Auth
{
    public class WebAuthenticationContext : IAuthenticationContext
    {
        public IPrincipal GenerateContextUser(string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userId)
            };
            var id = new ClaimsIdentity(claims, "Basic");
            var principal = new ClaimsPrincipal(new[] {id});

            return principal;
        }

        public void SetContextUser(string userId)
        {
            HttpContext.Current.User = GenerateContextUser(userId);
        }

        public void RemoveContextUser()
        {
            throw new System.NotImplementedException();
        }

        public string GetContextUser()
        {
            var claimsPrincipal = (HttpContext.Current.User as ClaimsPrincipal);
            if (claimsPrincipal == null || claimsPrincipal.Identities == null)
                return null;

            var name = claimsPrincipal.Identity.Name;

            if (name == "")
                name = null;

            return name;
        }
    }
}
