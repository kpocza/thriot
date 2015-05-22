using System.Security.Principal;

namespace Thriot.Management.Services
{
    public interface IAuthenticationContext
    {
        void SetUserPrincipalContext(IUserPrincipalContext userPrincipalContext);

        IPrincipal BuildContextUserPrincipal(string userId);

        void SetContextUser(string userId);

        void RemoveContextUser();

        string GetContextUser();
    }
}