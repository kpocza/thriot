using System;
using System.Data.Common;
using System.IO;
using IoT.Management.Operations.Sql.DataAccess;
using IoT.Management.Operations.Sql.DataAccess.Sql;
using IoT.Objects.Operations.Sql.DataAccess;
using IoT.Objects.Operations.Sql.DataAccess.Sql;
using IoT.ServiceClient.Messaging;

namespace IoT.UnitTestHelpers
{
    public class SqLiteEnvironmentFactory : IEnvironmentFactory
    {
        private IManagementUnitOfWorkFactory GetManagementUnitOfWorkFactory()
        {
            var sqliteConnectionParametersResolver = new SqLiteConnectionParametersResolver();

            InitializeDb(sqliteConnectionParametersResolver);

            return new ManagementUnitOfWorkFactorySql(sqliteConnectionParametersResolver);
        }

        private static bool _isInitialized = false;

        private static void InitializeDb(SqLiteConnectionParametersResolver sqliteConnectionParametersResolver)
        {
            if (_isInitialized)
                return;

            if(File.Exists("tmp.db"))
                File.Delete("tmp.db");

            var sql = File.ReadAllText("sqliteiotdb.sql");

            var statements = sql.Split(new[] {"\r\nGO\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            var dbProviderFactory =
                DbProviderFactories.GetFactory(sqliteConnectionParametersResolver.ManagementConnectionProvider);

            using (var dbConnection = dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = sqliteConnectionParametersResolver.ManagementConnectionString;

                dbConnection.Open();

                foreach (var stmt in statements)
                {
                    using (var dbCommand = dbProviderFactory.CreateCommand())
                    {
                        dbCommand.Connection = dbConnection;
                        dbCommand.CommandText = stmt;

                        dbCommand.ExecuteNonQuery();
                    }
                }
            }

            _isInitialized = true;
        }
        private IObjectsUnitOfWorkFactory GetPlatformUnitOfWorkFactory()
        {
            var connectionParameterResolver = new SqLiteConnectionParametersResolver();

            return new ObjectsUnitOfWorkFactorySql(connectionParameterResolver);
        }

        public Management.Model.Operations.IUserOperations MgmtUserOperations
        {
            get { return new Management.Operations.Sql.UserOperations(GetManagementUnitOfWorkFactory()); }
        }

        public Management.Model.Operations.ICompanyOperations MgmtCompanyOperations
        {
            get { return new Management.Operations.Sql.CompanyOperations(GetManagementUnitOfWorkFactory()); }
        }

        public Management.Model.Operations.IServiceOperations MgmtServiceOperations
        {
            get { return new Management.Operations.Sql.ServiceOperations(GetManagementUnitOfWorkFactory()); }
        }

        public Management.Model.Operations.INetworkOperations MgmtNetworkOperations
        {
            get { return new Management.Operations.Sql.NetworkOperations(GetManagementUnitOfWorkFactory()); }
        }

        public Management.Model.Operations.IDeviceOperations MgmtDeviceOperations
        {
            get { return new Management.Operations.Sql.DeviceOperations(GetManagementUnitOfWorkFactory()); }
        }

        public Management.Model.Operations.ISettingOperations MgmtSettingOperations
        {
            get { return new Management.Operations.Sql.SettingOperations(GetManagementUnitOfWorkFactory()); }
        }

        public Objects.Model.Operations.ICompanyOperations ObjCompanyOperations
        {
            get { return new Objects.Operations.Sql.CompanyOperations(GetPlatformUnitOfWorkFactory()); }
        }

        public Objects.Model.Operations.IServiceOperations ObjServiceOperations
        {
            get { return new Objects.Operations.Sql.ServiceOperations(GetPlatformUnitOfWorkFactory()); }
        }

        public Objects.Model.Operations.INetworkOperations ObjNetworkOperations
        {
            get { return new Objects.Operations.Sql.NetworkOperations(GetPlatformUnitOfWorkFactory()); }
        }

        public Objects.Model.Operations.IDeviceOperations ObjDeviceOperations
        {
            get { return new Objects.Operations.Sql.DeviceOperations(GetPlatformUnitOfWorkFactory()); }
        }

        public Objects.Model.Operations.ISettingOperations ObjSettingOperations
        {
            get { return new Objects.Operations.Sql.SettingOperations(GetPlatformUnitOfWorkFactory()); }
        }

        public IMessagingService MessagingService
        {
            get { return InMemoryMessagingService.Instance; }
        }
    }
}