using Thriot.Framework.Azure.DataAccess;
using Thriot.Plugins.Core;
using Thriot.Messaging.Services.Client;
using Thriot.TestHelpers.InMemoryQueue;
using Thriot.TestHelpers.InMemoryStorage;

namespace Thriot.TestHelpers
{
    public class InMemoryEnvironmentFactory : IEnvironmentFactory
    {
        private ICloudStorageClientFactory GetCloudStorageClientFactory()
        {
            return new InMemoryCloudStorageClientFactory();
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

        public IMessagingServiceClient MessagingServiceClient => InMemoryMessagingService.Instance;

        public string TelemetryConnectionString => null;

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent => null;

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries => null;

        public IQueueSendAdapter QueueSendAdapter => new InMemoryQueueSendAdapter();

        public IQueueReceiveAdapter QueueReceiveAdapter => new InMemorySerialQueueReceiveAdapter();
    }
}