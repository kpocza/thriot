using Thriot.Framework.Azure.DataAccess;

namespace Thriot.Plugins.Azure
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
