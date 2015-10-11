using Thriot.Management.Operations.Sql.DataAccess;
using Thriot.Management.Operations.Sql.DataAccess.PgSql;
using Thriot.Objects.Operations.Sql.DataAccess;
using Thriot.Objects.Operations.Sql.DataAccess.PgSql;

namespace Thriot.TestHelpers.Environments.Management
{
    public class PgSqlManagementEnvironment : IManagementEnvironment
    {
        private IManagementUnitOfWorkFactory GetManagementUnitOfWorkFactory()
        {
            var connectionParameterResolver = new DevPgSqlConnectionParametersResolver();

            return new ManagementUnitOfWorkFactoryPgSql(connectionParameterResolver);
        }

        private IObjectsUnitOfWorkFactory GetPlatformUnitOfWorkFactory()
        {
            var connectionParameterResolver = new DevPgSqlConnectionParametersResolver();

            return new ObjectsUnitOfWorkFactoryPgSql(connectionParameterResolver);
        }

        public Thriot.Management.Model.Operations.IUserOperations MgmtUserOperations => new Thriot.Management.Operations.Sql.UserOperations(GetManagementUnitOfWorkFactory());

        public Thriot.Management.Model.Operations.ICompanyOperations MgmtCompanyOperations => new Thriot.Management.Operations.Sql.CompanyOperations(GetManagementUnitOfWorkFactory());

        public Thriot.Management.Model.Operations.IServiceOperations MgmtServiceOperations => new Thriot.Management.Operations.Sql.ServiceOperations(GetManagementUnitOfWorkFactory());

        public Thriot.Management.Model.Operations.INetworkOperations MgmtNetworkOperations => new Thriot.Management.Operations.Sql.NetworkOperations(GetManagementUnitOfWorkFactory());

        public Thriot.Management.Model.Operations.IDeviceOperations MgmtDeviceOperations => new Thriot.Management.Operations.Sql.DeviceOperations(GetManagementUnitOfWorkFactory());

        public Thriot.Management.Model.Operations.ISettingOperations MgmtSettingOperations => new Thriot.Management.Operations.Sql.SettingOperations(GetManagementUnitOfWorkFactory());

        public Objects.Model.Operations.ICompanyOperations ObjCompanyOperations => new Objects.Operations.Sql.CompanyOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.IServiceOperations ObjServiceOperations => new Objects.Operations.Sql.ServiceOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.INetworkOperations ObjNetworkOperations => new Objects.Operations.Sql.NetworkOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.IDeviceOperations ObjDeviceOperations => new Objects.Operations.Sql.DeviceOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.ISettingOperations ObjSettingOperations => new Objects.Operations.Sql.SettingOperations(GetPlatformUnitOfWorkFactory());
    }
}
