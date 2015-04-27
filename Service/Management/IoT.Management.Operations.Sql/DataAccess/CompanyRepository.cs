using System.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public class CompanyRepository : Repository<Company>
    {
        public CompanyRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
