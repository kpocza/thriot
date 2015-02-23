using System.Linq;
using System.Security.Authentication;
using IoT.Framework.Exceptions;
using IoT.Management.Model.Operations;

namespace IoT.Management.Services
{
    public abstract class ManagementServiceBase
    {
        protected readonly ICompanyOperations _companyOperations;
        protected readonly IAuthenticationContext _authenticationContext;
        protected string _userId;

        protected ManagementServiceBase(ICompanyOperations companyOperations, IAuthenticationContext authenticationContext)
        {
            _companyOperations = companyOperations;
            _authenticationContext = authenticationContext;
        }

        protected void Authenticate()
        {
            _userId = _authenticationContext.GetContextUser();
            if (_userId == null)
                throw new AuthenticationException();
        }
        protected void AuthorizeCompany(string companyId)
        {
            var users = _companyOperations.ListUsers(companyId);

            if (users.All(u => u.Id != _userId))
                throw new ForbiddenException();
        }

    }
}
