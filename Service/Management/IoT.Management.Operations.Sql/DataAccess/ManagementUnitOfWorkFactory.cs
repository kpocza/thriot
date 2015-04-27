using Thriot.Framework.DataAccess;

namespace Thriot.Management.Operations.Sql.DataAccess
{
    public abstract class ManagementUnitOfWorkFactory : IManagementUnitOfWorkFactory
    {
        private readonly string _connectionString;
        private readonly string _connectionProvider;

        protected ManagementUnitOfWorkFactory(IConnectionParametersResolver connectionParametersResolver)
        {
            _connectionString = connectionParametersResolver.ManagementConnectionString;
            _connectionProvider = connectionParametersResolver.ManagementConnectionProvider;
        }

        public IManagementUnitOfWork Create()
        {
            var unitOfWork = CreateCore();
            unitOfWork.Setup(_connectionString, _connectionProvider);

            return unitOfWork;
        }

        public abstract IManagementUnitOfWork CreateCore();
    }
}
