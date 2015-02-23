using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public class DeviceRepository : Repository<Device>
    {
        public DeviceRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
