using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using Thriot.Management.Services;

namespace Thriot.Management.WebApi.Auth
{
    public class WebAuthenticationContext : IAuthenticationContext
    {
        private IUserPrincipalContext _userPrincipalContext;

        public void SetUserPrincipalContext(IUserPrincipalContext userPrincipalContext)
        {
            _userPrincipalContext = userPrincipalContext;
        }

        public IPrincipal BuildContextUserPrincipal(string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userId)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie");
            var principal = new ClaimsPrincipal(new[] {id});

            return principal;
        }

        public void SetContextUser(string userId)
        {
            var principal = BuildContextUserPrincipal(userId);
            _userPrincipalContext.User = principal;
            ((ApiController) _userPrincipalContext).Request.GetOwinContext().Authentication.SignIn(((ClaimsPrincipal) principal).Identity as ClaimsIdentity);
        }

        public void RemoveContextUser()
        {
            ((ApiController)_userPrincipalContext).Request.GetOwinContext().Authentication.SignOut();
        }

        public string GetContextUser()
        {
            var claimsPrincipal = (_userPrincipalContext.User as ClaimsPrincipal);
            if (claimsPrincipal == null || claimsPrincipal.Identities == null)
                return null;

            var name = claimsPrincipal.Identity.Name;

            if (name == "")
                name = null;

            return name;
        }
    }
}
