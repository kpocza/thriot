using System.Data.Entity;
using IoT.Framework.Sql;
using IoT.Objects.Model;

namespace IoT.Objects.Operations.Sql.DataAccess
{
    public class NetworkRepository : Repository<Network>
    {
        public NetworkRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
