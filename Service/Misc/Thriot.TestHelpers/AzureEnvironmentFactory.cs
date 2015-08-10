using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;
using Thriot.Plugins.Azure;
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
            get { return InprocMessagingServiceClient.Instance; }
        }

        public string TelemetryConnectionString
        {
            get { return "UseDevelopmentStorage=true"; }
        }

        public ITelemetryDataSinkCurrent TelemetryDataSinkCurrent
        {
            get { return new TelemetryDataSinkCurrent(); }
        }

        public ITelemetryDataSinkTimeSeries TelemetryDataSinkTimeSeries
        {
            get { return new TelemetryDataSinkTimeSeries(); }
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