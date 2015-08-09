using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Thriot.Framework.Azure.DataAccess;

namespace Thriot.Framework.Azure.TableOperations
{
    public interface ITableEntityOperation
    {
        void Insert(string tableName, TableEntity tableEntity);

        TE Get<TE>(string tableName, PartionKeyRowKeyPair paritionKeyRowKeyPair)
            where TE : TableEntity;

        void Update(string tableName, TableEntity tableEntity);

        void Upsert(string tableName, TableEntity tableEntity);

        void Delete(string tableName, TableEntity tableEntity);

        void EnsureTable(string tableName);

        IEnumerable<TE> QueryPartition<TE>(string tableName, string partitionKey)
            where TE : TableEntity, new();
    }
}
