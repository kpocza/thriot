using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public class ServiceRepository : Repository<Service>
    {
        public ServiceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
