using Microsoft.WindowsAzure.Storage.Table;
using Thriot.Framework.Azure.DataAccess;

namespace Thriot.Management.Operations.Azure.DataAccess
{
    public class DeviceTableEntity : PreparableTableEntity
    {
        public string Name { get; set; }

        public string ServiceId { get; set; }

        public string CompanyId { get; set; }

        public string NetworkId { get; set; }

        public string DeviceKey { get; set; }

        public long NumericId { get; set; }

        public DeviceTableEntity()
        {
            
        }

        public DeviceTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, string name, string networkId, string serviceId, string companyId, string deviceKey, long numericId)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Name = name;
            NetworkId = networkId;
            ServiceId = serviceId;
            CompanyId = companyId;
            DeviceKey = deviceKey;
            NumericId = numericId;
        }
    }
}
