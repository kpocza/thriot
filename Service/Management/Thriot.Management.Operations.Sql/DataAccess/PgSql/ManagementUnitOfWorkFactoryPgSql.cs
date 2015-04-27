using Thriot.Framework.DataAccess;

namespace Thriot.Management.Operations.Sql.DataAccess.PgSql
{
    public class ManagementUnitOfWorkFactoryPgSql : ManagementUnitOfWorkFactory
    {
        public ManagementUnitOfWorkFactoryPgSql(IConnectionParametersResolver connectionParametersResolver) : base(connectionParametersResolver)
        {
        }

        public override IManagementUnitOfWork CreateCore()
        {
            return new ManagementUnitOfWorkPgSql();
        }
    }
}
