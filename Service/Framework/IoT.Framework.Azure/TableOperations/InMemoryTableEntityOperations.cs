﻿using System.Collections.Generic;
using System.Linq;
using IoT.Framework.Azure.DataAccess;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoT.Framework.Azure.TableOperations
{
    public class InMemoryTableEntityOperations : ITableEntityOperation
    {
        public void Insert<TE>(string tableName, TableEntity tableEntity) where TE : TableEntity
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

        public void Update<TE>(string tableName, TableEntity tableEntity) where TE : TableEntity
        {
            InMemoryDatabase.Instance.Get(tableName).Update(tableEntity);
        }

        public void Upsert<TE>(string tableName, TableEntity tableEntity) where TE : TableEntity
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
