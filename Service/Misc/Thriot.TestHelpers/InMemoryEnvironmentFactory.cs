using Thriot.Framework.Azure.DataAccess;
using Thriot.Plugins.Core;
using Thriot.Messaging.Services.Client;

namespace Thriot.TestHelpers
{
    public class InMemoryEnvironmentFactory : IEnvironmentFactory
    {
        private ICloudStorageClientFactory GetCloudStorageClientFactory()
        {
            return new InMemoryCloudStorageClientFactory();
        }

        public Management.Model.Operations.IUserOperations MgmtUserOperations
        {
            get { return new Management.Operations.Azure.UserOperations(GetCloudStorageClientFactory()); }
        }

        public Management.Model.Operations.ICompanyOperations MgmtCompanyOperations
        {
            get { return new Management.Operations.Azure.CompanyOperations(GetCloudStorageClientFactory()); }
        }

        public Management.Model.Operations.IServiceOperations MgmtServiceOperations
        {
            get { return new Management.Operations.Azure.ServiceOperations(GetCloudStorageClientFactory()); }
        }

        public Management.Model.Operations.INetworkOperations MgmtNetworkOperations
        {
            get { return new Management.Operations.Azure.NetworkOperations(GetCloudStorageClientFactory()); }
        }

        public Management.Model.Operations.IDeviceOperations MgmtDeviceOperations
        {
            get { return new Management.Operations.Azure.DeviceOperations(GetCloudStorageClientFactory()); }
        }

        public Management.Model.Operations.ISettingOperations MgmtSettingOperations
        {
            get { return new Management.Operations.Azure.SettingOperations(GetCloudStorageClientFactory()); }
        }

        public Objects.Model.Operations.ICompanyOperations ObjCompanyOperations
        {
            get { return new Objects.Operations.Azure.CompanyOperations(GetCloudStorageClientFactory()); }
        }

        public Objects.Model.Operations.IServiceOperations ObjServiceOperations
        {
            get { return new Objects.Operations.Azure.ServiceOperations(GetCloudStorageClientFactory()); }
        }

        public Objects.Model.Operations.INetworkOperations ObjNetworkOperations
        {
            get { return new Objects.Operations.Azure.NetworkOperations(GetCloudStorageClientFactory()); }
        }

        public Objects.Model.Operations.IDeviceOperations ObjDeviceOperations
        {
            get { return new Objects.Operations.Azure.DeviceOperations(GetCloudStorageClientFactory()); }
        }

        public Objects.Model.Operations.ISettingOperations ObjSettingOperations
        {
            get { return new Objects.Operations.Azure.SettingOperations(GetCloudStorageClientFactory()); }
        }

        public IMessagingServiceClient MessagingServiceClient
        {
            get { return InMemoryMessagingService.Instance; }
        }

        public string TelemetryConnectionString
        {
            get { return null; }
        }

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent
        {
            get { return null; }
        }

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries
        {
            get { return null; }
        }
    }
}