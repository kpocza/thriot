using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Framework.Azure.DataAccess
{
    public class InMemoryCloudStorageClientFactory : ICloudStorageClientFactory
    {
        public ITableEntityOperation GetTableEntityOperation()
        {
            return new InMemoryTableEntityOperations();
        }
    }
}