using IoT.Framework.Azure.TableOperations;

namespace IoT.Framework.Azure.DataAccess
{
    public interface ICloudStorageClientFactory
    {
        ITableEntityOperation GetTableEntityOperation();
    }
}
