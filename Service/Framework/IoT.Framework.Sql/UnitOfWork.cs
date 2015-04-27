using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;

namespace Thriot.Framework.Sql
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected DbContext DbContext;

        protected abstract DbContext GetDbContext(string connectionString, string providerName);

        public void Setup(string connectionString, string providerName)
        {
            DbContext = GetDbContext(connectionString, providerName);
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
