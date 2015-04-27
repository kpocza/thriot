using Thriot.Framework.DataAccess;

namespace Thriot.Objects.Operations.Sql.DataAccess.PgSql
{
    public class ObjectsUnitOfWorkFactoryPgSql : ObjectsUnitOfWorkFactory
    {
        public ObjectsUnitOfWorkFactoryPgSql(IConnectionParametersResolver connectionParametersResolver)
            : base(connectionParametersResolver)
        {
        }

        protected override ObjectsUnitOfWork CreateCore()
        {
            return new ObjectsUnitOfWorkPgSql();
        }
    }
}
