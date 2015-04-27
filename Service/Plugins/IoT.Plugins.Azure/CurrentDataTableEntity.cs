using System;
using Thriot.Framework.Azure.DataAccess;

namespace Thriot.Plugins.Azure
{
    public class CurrentDataTableEntity : PreparableTableEntity
    {
        public DateTime Time { get; set; }

        public string Payload { get; set; }
        
        public CurrentDataTableEntity()
        {
            
        }

        public CurrentDataTableEntity(PartionKeyRowKeyPair partitionKeyRowKeyPair, DateTime time, string payload)
        {
            PartitionKey = partitionKeyRowKeyPair.PartitionKey;
            RowKey = partitionKeyRowKeyPair.RowKey;

            Time = time;
            Payload = payload;
        }
    }
}
