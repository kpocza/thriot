﻿using System.Data.Common;

namespace Thriot.Management.Operations.Sql.DataAccess.PgSql
{
    public class ManagementUnitOfWorkPgSql : ManagementUnitOfWork
    {
        protected override ManagementDbContext GetDbContextCore(DbConnection dbConnection, bool ownsConnections)
        {
            return new ManagementDbContextPgSql(dbConnection, ownsConnections);
        }
    }
}
