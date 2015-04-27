using System;
using System.Threading.Tasks;

namespace Thriot.Framework.Sql
{
    public interface IUnitOfWork : IDisposable
    {
        void Setup(string connectionString, string providerName);

        void Commit();

        Task CommitAsync();
    }
}
