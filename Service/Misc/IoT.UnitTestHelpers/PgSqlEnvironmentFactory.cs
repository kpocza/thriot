﻿using IoT.Management.Operations.Sql.DataAccess;
using IoT.Management.Operations.Sql.DataAccess.PgSql;
using IoT.Objects.Operations.Sql.DataAccess;
using IoT.Objects.Operations.Sql.DataAccess.PgSql;
using IoT.Plugins.Core;
using IoT.ServiceClient.Messaging;

namespace IoT.UnitTestHelpers
{
    public class PgSqlEnvironmentFactory : IEnvironmentFactory
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
            get { return InprocMessagingService.Instance; }
        }

        public string TelemetryConnectionString
        {
            get
            {
                return
                    "Server=127.0.0.1;Port=5432;Database=ThriotTelemetry;User Id=thriottelemetry;Password=thriottelemetry;";
            }
        }

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent
        {
            get { return new IoT.Plugins.PgSql.TelemetryDataSinkCurrent(); }
        }

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries
        {
            get { return new IoT.Plugins.PgSql.TelemetryDataSinkTimeSeries(); }
        }
    }
} 