using System.Collections.Generic;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Plugins.Core;
using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers
{
    public class AzureEnvironmentFactory : IEnvironmentFactory
    {
        public ICloudStorageClientFactory GetCloudStorageClientFactory()
        {
            var cloudStorageClientFactory = new AzureCloudStorageClientFactory(new DevAzureConnectionParametersResolver());

            EnsureTables(cloudStorageClientFactory.GetTableEntityOperation());

            return cloudStorageClientFactory;
        }

        public Management.Model.Operations.IUserOperations MgmtUserOperations => new Management.Operations.Azure.UserOperations(GetCloudStorageClientFactory());

        public Management.Model.Operations.ICompanyOperations MgmtCompanyOperations => new Management.Operations.Azure.CompanyOperations(GetCloudStorageClientFactory());

        public Management.Model.Operations.IServiceOperations MgmtServiceOperations => new Management.Operations.Azure.ServiceOperations(GetCloudStorageClientFactory());

        public Management.Model.Operations.INetworkOperations MgmtNetworkOperations => new Management.Operations.Azure.NetworkOperations(GetCloudStorageClientFactory());

        public Management.Model.Operations.IDeviceOperations MgmtDeviceOperations => new Management.Operations.Azure.DeviceOperations(GetCloudStorageClientFactory());

        public Management.Model.Operations.ISettingOperations MgmtSettingOperations => new Management.Operations.Azure.SettingOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.ICompanyOperations ObjCompanyOperations => new Objects.Operations.Azure.CompanyOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.IServiceOperations ObjServiceOperations => new Objects.Operations.Azure.ServiceOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.INetworkOperations ObjNetworkOperations => new Objects.Operations.Azure.NetworkOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.IDeviceOperations ObjDeviceOperations => new Objects.Operations.Azure.DeviceOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.ISettingOperations ObjSettingOperations => new Objects.Operations.Azure.SettingOperations(GetCloudStorageClientFactory());

        public IMessagingServiceClient MessagingServiceClient => InprocMessagingServiceClient.Instance;

        public string TelemetryConnectionString => "UseDevelopmentStorage=true";

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent => InstanceCreator.Create<ITelemetryDataSinkCurrent>("Thriot.Plugins.Azure.TelemetryDataSinkCurrent, Thriot.Plugins.Azure");

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries => InstanceCreator.Create<ITelemetryDataSinkTimeSeries>("Thriot.Plugins.Azure.TelemetryDataSinkTimeSeries, Thriot.Plugins.Azure");

        public string QueueConnectionString => "UseDevelopmentStorage=true";

        public IQueueSendAdapter QueueSendAdapter
        {
            get
            {
                var queueSendAdapter = InstanceCreator.Create<IQueueSendAdapter>("Thriot.Plugins.Azure.QueueSendAdapter, Thriot.Plugins.Azure");
                queueSendAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString }, { "QueueName", "telemetry" } });
                return queueSendAdapter;
            }
        } 

        public IQueueReceiveAdapter QueueReceiveAdapter
        {
            get
            {
                var queueReceiveAdapter = InstanceCreator.Create<IQueueReceiveAdapter>("Thriot.Plugins.Azure.QueueReceiveAdapter, Thriot.Plugins.Azure");
                queueReceiveAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString }, { "QueueName", "telemetry" } });
                return queueReceiveAdapter;
            }
        }

        private static volatile bool _created = false;

        private void EnsureTables(ITableEntityOperation tableEntityOperations)
        {
            if (!_created)
            {
                // for management
                tableEntityOperations.EnsureTable("LoginUser");
                tableEntityOperations.EnsureTable("User");
                tableEntityOperations.EnsureTable("Company");
                tableEntityOperations.EnsureTable("Service");
                tableEntityOperations.EnsureTable("Network");
                tableEntityOperations.EnsureTable("Device");
                tableEntityOperations.EnsureTable("Setting");

                // for data
                tableEntityOperations.EnsureTable("CurrentData");
                tableEntityOperations.EnsureTable("TimeSeries");
            }
            _created = true;
        }
    }
}