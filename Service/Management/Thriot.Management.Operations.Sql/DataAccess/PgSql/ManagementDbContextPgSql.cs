using Microsoft.Data.Entity;
using Thriot.Management.Model;

namespace Thriot.Management.Operations.Sql.DataAccess.PgSql
{
    public class ManagementDbContextPgSql : ManagementDbContext
    {
        private readonly string _connectionString;

        public ManagementDbContextPgSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
//            modelBuilder.HasDefaultSchema("public");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Device>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Network>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<Service>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<User>().Property(p => p.Id).HasColumnType("varchar");//.IsFixedLength();
            modelBuilder.Entity<LoginUser>().Property(p => p.UserId).HasColumnType("varchar");//.IsFixedLength();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
