using Microsoft.Data.Entity;
using Thriot.Framework.Sql;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public class NetworkRepository : Repository<Network>
    {
        public NetworkRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
