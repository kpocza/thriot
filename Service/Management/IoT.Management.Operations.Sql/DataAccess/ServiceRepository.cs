using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Management.Model;

namespace IoT.Management.Operations.Sql.DataAccess
{
    public class ServiceRepository : Repository<Service>
    {
        public ServiceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
