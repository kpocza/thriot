using System.Data.Common;
using System.Data.Entity;

namespace IoT.Management.Operations.Sql.DataAccess.Sql
{
    public class ManagementDbContextSql : ManagementDbContext
    {
        public ManagementDbContextSql(DbConnection dbConnection, bool ownsConnections)
            : base(dbConnection, ownsConnections)
        {
            Database.SetInitializer<ManagementDbContextSql>(null);
            
            // dirty hack to make unit tests work
            // ensure that ef dlls are copied to the right place
            var _ = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }

        public ManagementDbContextSql()
        {
            
        }
    }
}
