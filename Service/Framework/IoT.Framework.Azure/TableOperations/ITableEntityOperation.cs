using System.Collections.Generic;
using IoT.Framework.Azure.DataAccess;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoT.Framework.Azure.TableOperations
{
    public interface ITableEntityOperation
    {
        void Insert<TE>(string tableName, TableEntity tableEntity)
            where TE : TableEntity;

        TE Get<TE>(string tableName, PartionKeyRowKeyPair paritionKeyRowKeyPair)
            where TE : TableEntity;

        void Update<TE>(string tableName, TableEntity tableEntity)
            where TE : TableEntity;

        void Upsert<TE>(string tableName, TableEntity tableEntity)
            where TE : TableEntity;

        void Delete(string tableName, TableEntity tableEntity);

        void EnsureTable(string tableName);

        IEnumerable<TE> QueryPartition<TE>(string tableName, string partitionKey)
            where TE : TableEntity, new();
    }
}
