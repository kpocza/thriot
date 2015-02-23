using System.Security.Principal;

namespace IoT.Management.Services
{
    public interface IAuthenticationContext
    {
        IPrincipal GenerateContextUser(string userId);

        void SetContextUser(string userId);

        void RemoveContextUser();

        string GetContextUser();
    }
}