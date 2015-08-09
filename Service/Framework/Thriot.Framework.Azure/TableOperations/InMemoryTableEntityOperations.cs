using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Thriot.Framework.Azure.DataAccess;

namespace Thriot.Framework.Azure.TableOperations
{
    public class InMemoryTableEntityOperations : ITableEntityOperation
    {
        public void Insert(string tableName, TableEntity tableEntity)
        {
            InMemoryDatabase.Instance.Get(tableName).Add(tableEntity);
        }

        public TE Get<TE>(string tableName, PartionKeyRowKeyPair partionKeyRowKeyPair) where TE : TableEntity
        {
            var entity = InMemoryDatabase.Instance.Get(tableName).Get(partionKeyRowKeyPair);

            if (entity == null)
                return null;

            return Serializers.FromJsonString<TE>(entity);
        }

        public void Update(string tableName, TableEntity tableEntity)
        {
            InMemoryDatabase.Instance.Get(tableName).Update(tableEntity);
        }

        public void Upsert(string tableName, TableEntity tableEntity)
        {
            InMemoryDatabase.Instance.Get(tableName).Upsert(tableEntity);
        }

        public void Delete(string tableName, TableEntity tableEntity)
        {
            InMemoryDatabase.Instance.Get(tableName).Delete(tableEntity);
        }

        public IEnumerable<TE> QueryPartition<TE>(string tableName, string partitionKey) where TE : TableEntity, new()
        {
            var strings = InMemoryDatabase.Instance.Get(tableName).Get(partitionKey);

            return strings.Select(Serializers.FromJsonString<TE>).ToList();
        }

        public void EnsureTable(string tableName)
        {
            InMemoryDatabase.Instance.Get(tableName);
        }
    }
}
