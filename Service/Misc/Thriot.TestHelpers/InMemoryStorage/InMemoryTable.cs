using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Thriot.Framework;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.DataAccess;

namespace Thriot.TestHelpers.InMemoryStorage
{
    public class InMemoryTable
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _entities;

        public InMemoryTable()
        {
            _entities = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        }

        public void Add(TableEntity tableEntity)
        {
            var partition = _entities.GetOrAdd(tableEntity.PartitionKey, new ConcurrentDictionary<string, string>());
            
            string oldValue;
            if(partition.TryGetValue(tableEntity.RowKey, out oldValue) && oldValue!= null)
                throw new StorageAccessException(HttpStatusCode.Conflict);

            partition.TryAdd(tableEntity.RowKey, Serializers.ToJsonString(tableEntity));
        }

        public void Update(TableEntity tableEntity)
        {
            var partition = _entities.GetOrAdd(tableEntity.PartitionKey, new ConcurrentDictionary<string, string>());

            string oldValue;
            if(!partition.TryGetValue(tableEntity.RowKey, out oldValue))
                throw new StorageAccessException(HttpStatusCode.NotFound);

            partition.TryUpdate(tableEntity.RowKey, Serializers.ToJsonString(tableEntity), oldValue);
        }

        public void Upsert(TableEntity tableEntity)
        {
            var partition = _entities.GetOrAdd(tableEntity.PartitionKey, new ConcurrentDictionary<string, string>());

            partition[tableEntity.RowKey] = Serializers.ToJsonString(tableEntity);
        }

        public void Delete(TableEntity tableEntity)
        {
            var partition = _entities.GetOrAdd(tableEntity.PartitionKey, new ConcurrentDictionary<string, string>());

            string oldValue;
            partition.TryRemove(tableEntity.RowKey, out oldValue);
        }

        public string Get(PartionKeyRowKeyPair partitionKeyRowKeyPair)
        {
            var partition = _entities.GetOrAdd(partitionKeyRowKeyPair.PartitionKey, new ConcurrentDictionary<string, string>());

            string currentValue;
            partition.TryGetValue(partitionKeyRowKeyPair.RowKey, out currentValue);

            return currentValue;
        }

        public IEnumerable<string> Get(string partitionKey)
        {
            var partition = _entities.GetOrAdd(partitionKey, new ConcurrentDictionary<string, string>());

            return partition.Values;
        }
    }
}
