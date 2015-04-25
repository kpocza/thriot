using System.Data.Common;
using System.Data.Entity;

namespace IoT.Objects.Operations.Sql.DataAccess.Sql
{
    public class ObjectsDbContextSql : ObjectsDbContext
    {
        public ObjectsDbContextSql(DbConnection dbConnection, bool ownsConnections) : base(dbConnection, ownsConnections)
        {
            Database.SetInitializer<ObjectsDbContextSql>(null);

            // dirty hack to make unit tests work
            // ensure that ef dlls are copied to the right place
            var _ = typeof (System.Data.Entity.SqlServer.SqlProviderServices);
        }

        public ObjectsDbContextSql()
        {
        }
    }
}
