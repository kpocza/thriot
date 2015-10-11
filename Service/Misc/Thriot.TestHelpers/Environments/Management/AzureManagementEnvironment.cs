using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.TestHelpers.Environments.Management
{
    public class AzureManagementEnvironment : IManagementEnvironment
    {
        public ICloudStorageClientFactory GetCloudStorageClientFactory()
        {
            var cloudStorageClientFactory = new AzureCloudStorageClientFactory(new DevAzureConnectionParametersResolver());

            EnsureTables(cloudStorageClientFactory.GetTableEntityOperation());

            return cloudStorageClientFactory;
        }

        public Thriot.Management.Model.Operations.IUserOperations MgmtUserOperations => new Thriot.Management.Operations.Azure.UserOperations(GetCloudStorageClientFactory());

        public Thriot.Management.Model.Operations.ICompanyOperations MgmtCompanyOperations => new Thriot.Management.Operations.Azure.CompanyOperations(GetCloudStorageClientFactory());

        public Thriot.Management.Model.Operations.IServiceOperations MgmtServiceOperations => new Thriot.Management.Operations.Azure.ServiceOperations(GetCloudStorageClientFactory());

        public Thriot.Management.Model.Operations.INetworkOperations MgmtNetworkOperations => new Thriot.Management.Operations.Azure.NetworkOperations(GetCloudStorageClientFactory());

        public Thriot.Management.Model.Operations.IDeviceOperations MgmtDeviceOperations => new Thriot.Management.Operations.Azure.DeviceOperations(GetCloudStorageClientFactory());

        public Thriot.Management.Model.Operations.ISettingOperations MgmtSettingOperations => new Thriot.Management.Operations.Azure.SettingOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.ICompanyOperations ObjCompanyOperations => new Objects.Operations.Azure.CompanyOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.IServiceOperations ObjServiceOperations => new Objects.Operations.Azure.ServiceOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.INetworkOperations ObjNetworkOperations => new Objects.Operations.Azure.NetworkOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.IDeviceOperations ObjDeviceOperations => new Objects.Operations.Azure.DeviceOperations(GetCloudStorageClientFactory());

        public Objects.Model.Operations.ISettingOperations ObjSettingOperations => new Objects.Operations.Azure.SettingOperations(GetCloudStorageClientFactory());


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
