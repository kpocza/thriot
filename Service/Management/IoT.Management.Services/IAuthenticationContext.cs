using System.Security.Principal;

namespace Thriot.Management.Services
{
    public interface IAuthenticationContext
    {
        IPrincipal GenerateContextUser(string userId);

        void SetContextUser(string userId);

        void RemoveContextUser();

        string GetContextUser();
    }
}