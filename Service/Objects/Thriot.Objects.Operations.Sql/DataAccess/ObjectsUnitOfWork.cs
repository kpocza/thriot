using System.Data.Common;
using Microsoft.Data.Entity;
using Thriot.Framework.Sql;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public abstract class ObjectsUnitOfWork : UnitOfWork, IObjectsUnitOfWork
    {
        protected override DbContext GetDbContext(string connectionString, string providerName)
        {
            return GetDbContextCore(connectionString);
        }

        protected abstract ObjectsDbContext GetDbContextCore(string connectionString);

        public CompanyRepository GetCompanyRepository()
        {
            return new CompanyRepository(DbContext);
        }

        public ServiceRepository GetServiceRepository()
        {
            return new ServiceRepository(DbContext);
        }

        public NetworkRepository GetNetworkRepository()
        {
            return new NetworkRepository(DbContext);
        }

        public DeviceRepository GetDeviceRepository()
        {
            return new DeviceRepository(DbContext);
        }

        public SettingRepository GetSettingRepository()
        {
            return new SettingRepository(DbContext);
        }
    }
}
