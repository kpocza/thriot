using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.Azure.TableOperations;

namespace Thriot.TestHelpers.InMemoryStorage
{
    public class InMemoryCloudStorageClientFactory : ICloudStorageClientFactory
    {
        public ITableEntityOperation GetTableEntityOperation()
        {
            return new InMemoryTableEntityOperations();
        }
    }
}