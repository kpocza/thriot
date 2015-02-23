using System;
using System.Threading.Tasks;

namespace IoT.Framework.Sql
{
    public interface IUnitOfWork : IDisposable
    {
        void Setup(string connectionString, string providerName, bool enableMigrations = true);

        void Commit();

        Task CommitAsync();
    }
}
