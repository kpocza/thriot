using System.Security.Principal;

namespace Thriot.Management.Services
{
    public interface IUserPrincipalContext
    {
        IPrincipal User { get; set; }
    }
}
