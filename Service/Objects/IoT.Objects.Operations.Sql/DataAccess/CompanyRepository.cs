using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public class CompanyRepository : Repository<Company>
    {
        public CompanyRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
