using System.Data.Entity;
using System.Threading.Tasks;

namespace IoT.Framework.Sql
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected DbContext DbContext;

        protected abstract DbContext GetDbContext(string connectionString, string providerName, bool enableMigrations);

        public void Setup(string connectionString, string providerName, bool enableMigrations = true)
        {
            DbContext = GetDbContext(connectionString, providerName, enableMigrations);
        }

        public void Commit()
        {
            DbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
