using System.Data.Common;

namespace Thriot.Objects.Operations.Sql.DataAccess.PgSql
{
    public class ObjectsUnitOfWorkPgSql : ObjectsUnitOfWork
    {
        protected override ObjectsDbContext GetDbContextCore(DbConnection dbConnection)
        {
            return new ObjectsDbContextPgSql(dbConnection, true);
        }
    }
}
