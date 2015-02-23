using IoT.Framework.Azure.TableOperations;
using IoT.Framework.DataAccess;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoT.Framework.Azure.DataAccess
{
    public class AzureCloudStorageClientFactory : ICloudStorageClientFactory
    {
        private readonly string _connectionString;

        public AzureCloudStorageClientFactory(IConnectionParametersResolver connectionParametersResolver)
        {
            _connectionString = connectionParametersResolver.ManagementConnectionString;
        }
        public ITableEntityOperation GetTableEntityOperation()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            return new AzureTableEntityOperations(tableClient);
        }
    }
}