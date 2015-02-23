using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public class ServiceRepository : Repository<Service>
    {
        public ServiceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
