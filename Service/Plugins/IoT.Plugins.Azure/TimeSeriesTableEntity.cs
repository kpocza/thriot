using IoT.Framework.Azure.DataAccess;

namespace IoT.Plugins.Azure
{
    public class TimeSeriesTableEntity : PreparableTableEntity
    {
        public string Payload { get; set; }
        
        public TimeSeriesTableEntity()
        {
            
        }

        public TimeSeriesTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, string payload)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Payload = payload;
        }
    }
}
