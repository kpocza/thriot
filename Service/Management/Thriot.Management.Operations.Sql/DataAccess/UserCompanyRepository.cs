using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public class UserCompanyRepository : GenericQueryRepository<UserCompany>
    {
        public UserCompanyRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
