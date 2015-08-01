using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public class ServiceRepository : Repository<Service>
    {
        public ServiceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
