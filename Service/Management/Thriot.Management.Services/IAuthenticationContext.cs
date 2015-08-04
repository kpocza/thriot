using System.Security.Principal;

namespace Thriot.Management.Services
{
    public interface IAuthenticationContext
    {
        void SetContextUser(string userId);

        void RemoveContextUser();

        string GetContextUser();
    }
}