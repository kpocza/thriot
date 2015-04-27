﻿using System.Data.Common;

namespace Thriot.Management.Operations.Sql.DataAccess.Sql
{
    public class ManagementUnitOfWorkSql : ManagementUnitOfWork
    {
        protected override ManagementDbContext GetDbContextCore(DbConnection dbConnection, bool ownsConnections)
        {
            return new ManagementDbContextSql(dbConnection, ownsConnections);
        }
    }
}
