using System.Data.Common;

namespace Thriot.Objects.Operations.Sql.DataAccess.Sql
{
    public class ObjectsUnitOfWorkSql : ObjectsUnitOfWork
    {
        protected override ObjectsDbContext GetDbContextCore(DbConnection dbConnection)
        {
            return new ObjectsDbContextSql(dbConnection, true);
        }
    }
}
