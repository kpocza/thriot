using System;
using Microsoft.Data.Entity;

namespace Thriot.Management.Operations.Sql.DataAccess.Sql
{
    public class ManagementDbContextSql : ManagementDbContext
    {
        private readonly string _connectionString;

        public ManagementDbContextSql(string connectionString)
        {
            _connectionString = connectionString;
//            Database.SetInitializer<ManagementDbContextSql>(null);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
