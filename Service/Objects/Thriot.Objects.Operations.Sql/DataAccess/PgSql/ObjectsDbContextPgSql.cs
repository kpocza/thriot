﻿using Microsoft.Data.Entity;
using Thriot.Objects.Model;

namespace Thriot.Objects.Operations.Sql.DataAccess.PgSql
{
    public class ObjectsDbContextPgSql : ObjectsDbContext
    {
        private readonly string _connectionString;

        public ObjectsDbContextPgSql(string connectionString)
        {
            _connectionString = connectionString;
//            Database.SetInitializer<ObjectsDbContextPgSql>(null);
        }

        public ObjectsDbContextPgSql()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
//            modelBuilder.HasDefaultSchema("public");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.CompanyId).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.ServiceId).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.NetworkId).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.CompanyId).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.ServiceId).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.CompanyId).HasColumnType("varchar");//.IsFixedLength();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
