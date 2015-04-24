using System.Data.Common;

namespace IoT.Management.Operations.Sql.DataAccess.Sql
{
    public class ManagementDbContextSql : ManagementDbContext
    {
        public ManagementDbContextSql(DbConnection dbConnection, bool ownsConnections)
            : base(dbConnection, ownsConnections)
        {
            // dirty hack to make unit tests work
            // ensure that ef dlls are copied to the right place
            var _ = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }

        public ManagementDbContextSql()
        {
            
        }
    }
}
