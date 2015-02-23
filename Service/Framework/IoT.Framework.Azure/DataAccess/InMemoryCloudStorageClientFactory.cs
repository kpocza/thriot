using IoT.Framework.Azure.TableOperations;

namespace IoT.Framework.Azure.DataAccess
{
    public class InMemoryCloudStorageClientFactory : ICloudStorageClientFactory
    {
        public ITableEntityOperation GetTableEntityOperation()
        {
            return new InMemoryTableEntityOperations();
        }
    }
}