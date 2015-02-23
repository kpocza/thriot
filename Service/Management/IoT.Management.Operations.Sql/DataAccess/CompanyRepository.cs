using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Management.Model;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class CompanyRepository : Repository<Company>
    {
        public CompanyRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
