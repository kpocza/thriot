using Thriot.Framework.DataAccess;

namespace Thriot.Objects.Operations.Sql.DataAccess
{
    public abstract class ObjectsUnitOfWorkFactory : IObjectsUnitOfWorkFactory
    {
        private readonly string _connectionString;
        private readonly string _connectionProvider;

        protected ObjectsUnitOfWorkFactory(IConnectionParametersResolver connectionParametersResolver)
        {
            _connectionString = connectionParametersResolver.ManagementConnectionString;
            _connectionProvider = connectionParametersResolver.ManagementConnectionProvider;
        }

        public IObjectsUnitOfWork Create()
        {
            var unitOfWork = CreateCore();
            unitOfWork.Setup(_connectionString, _connectionProvider);

            return unitOfWork;
        }

        protected abstract ObjectsUnitOfWork CreateCore();
    }
}
