using Thriot.Framework.Azure.TableOperations;

namespace Thriot.Framework.Azure.DataAccess
{
    public interface ICloudStorageClientFactory
    {
        ITableEntityOperation GetTableEntityOperation();
    }
}
