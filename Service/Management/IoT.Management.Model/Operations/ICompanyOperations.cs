using System.Collections.Generic;

namespace IoT.Management.Model.Operations
{
    public interface ICompanyOperations
    {
        string Create(Company company, string userId);

        Company Get(string id);

        void Update(Company company);

        void Delete(string id);

        IList<SmallUser> ListUsers(string companyId);

        IList<Small> ListServices(string companyId);

        void AddUser(string companyId, string userId);
    }
}