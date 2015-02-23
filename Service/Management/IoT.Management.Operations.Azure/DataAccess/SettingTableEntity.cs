using IoT.Framework.Azure.DataAccess;

namespace IoT.Management.Operations.Azure.DataAccess
{
    public class SettingTableEntity : PreparableTableEntity
    {
        public string Value { get; set; }

        public SettingTableEntity()
        {
        }

        public SettingTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, string value)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Value = value;
        }
    }
}
