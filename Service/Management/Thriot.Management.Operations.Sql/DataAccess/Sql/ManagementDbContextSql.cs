﻿using System;
using Microsoft.Data.Entity;

namespace Thriot.Management.Operations.Sql.DataAccess.Sql
{
    public class ManagementDbContextSql : ManagementDbContext
    {
        private readonly string _connectionString;

        public ManagementDbContextSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
