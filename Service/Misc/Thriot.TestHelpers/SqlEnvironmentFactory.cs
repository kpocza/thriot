using System.Collections.Generic;
using Thriot.Management.Operations.Sql.DataAccess;
using Thriot.Management.Operations.Sql.DataAccess.Sql;
using Thriot.Objects.Operations.Sql.DataAccess;
using Thriot.Objects.Operations.Sql.DataAccess.Sql;
using Thriot.Plugins.Core;
using Thriot.Plugins.Sql;
using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers
{
    public class SqlEnvironmentFactory : IEnvironmentFactory
    {
        private IManagementUnitOfWorkFactory GetManagementUnitOfWorkFactory()
        {
            var connectionParameterResolver = new DevSqlConnectionParametersResolver();

            return new ManagementUnitOfWorkFactorySql(connectionParameterResolver);
        }

        private IObjectsUnitOfWorkFactory GetPlatformUnitOfWorkFactory()
        {
            var connectionParameterResolver = new DevSqlConnectionParametersResolver();

            return new ObjectsUnitOfWorkFactorySql(connectionParameterResolver);
        }

        public Management.Model.Operations.IUserOperations MgmtUserOperations => new Management.Operations.Sql.UserOperations(GetManagementUnitOfWorkFactory());

        public Management.Model.Operations.ICompanyOperations MgmtCompanyOperations => new Management.Operations.Sql.CompanyOperations(GetManagementUnitOfWorkFactory());

        public Management.Model.Operations.IServiceOperations MgmtServiceOperations => new Management.Operations.Sql.ServiceOperations(GetManagementUnitOfWorkFactory());

        public Management.Model.Operations.INetworkOperations MgmtNetworkOperations => new Management.Operations.Sql.NetworkOperations(GetManagementUnitOfWorkFactory());

        public Management.Model.Operations.IDeviceOperations MgmtDeviceOperations => new Management.Operations.Sql.DeviceOperations(GetManagementUnitOfWorkFactory());

        public Management.Model.Operations.ISettingOperations MgmtSettingOperations => new Management.Operations.Sql.SettingOperations(GetManagementUnitOfWorkFactory());

        public Objects.Model.Operations.ICompanyOperations ObjCompanyOperations => new Objects.Operations.Sql.CompanyOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.IServiceOperations ObjServiceOperations => new Objects.Operations.Sql.ServiceOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.INetworkOperations ObjNetworkOperations => new Objects.Operations.Sql.NetworkOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.IDeviceOperations ObjDeviceOperations => new Objects.Operations.Sql.DeviceOperations(GetPlatformUnitOfWorkFactory());

        public Objects.Model.Operations.ISettingOperations ObjSettingOperations => new Objects.Operations.Sql.SettingOperations(GetPlatformUnitOfWorkFactory());

        public IMessagingServiceClient MessagingServiceClient => InprocMessagingServiceClient.Instance;

        public string TelemetryConnectionString => @"Server=.\SQLEXPRESS;Database=ThriotTelemetry;Trusted_Connection=True;";

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent => new TelemetryDataSinkCurrent();

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries => new TelemetryDataSinkTimeSeries();

        public string QueueConnectionString => @"Server=.\SQLEXPRESS;Database=ThriotTelemetryQueue;Trusted_Connection=True;";

        public IQueueSendAdapter QueueSendAdapter
        {
            get
            {
                var queueSendAdapter = new QueueSendAdapter();
                queueSendAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString } });
                return queueSendAdapter;
            }
        }

        public IQueueReceiveAdapter QueueReceiveAdapter
        {
            get
            {
                var queueReceiveAdapter = new QueueReceiveAdapter();
                queueReceiveAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString } });
                return queueReceiveAdapter;
            }
        }
    }
} 