using System.Collections.Generic;

namespace Thriot.Management.Model.Operations
{
    public interface IUserOperations
    {
        string Create(User user, string passwordHash, string salt);

        bool IsExists(string email);

        User Get(string id);

        void Update(User user);

        IList<Small> ListCompanies(string userIdentity);

        LoginUser GetLoginUser(string email);
    }
}
