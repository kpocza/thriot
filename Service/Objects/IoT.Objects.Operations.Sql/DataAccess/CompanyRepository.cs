using System.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public class CompanyRepository : Repository<Company>
    {
        public CompanyRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
