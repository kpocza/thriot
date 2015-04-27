using System.Collections.Generic;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Thriot.Framework.Azure.DataAccess;
using Thriot.Framework.DataAccess;

namespace Thriot.Framework.Azure.TableOperations
{
    public class AzureTableEntityOperations : ITableEntityOperation
    {
        private readonly CloudTableClient _tableClient;

        public AzureTableEntityOperations(CloudTableClient tableClient)
        {
            _tableClient = tableClient;
        }

        public void Insert<TE>(string tableName, TableEntity tableEntity)
            where TE : TableEntity
        {
            try
            {
                var result = GetTableReference(tableName).Execute(TableOperation.Insert(tableEntity));

                if (result.HttpStatusCode != (int) HttpStatusCode.NoContent)
                    throw new StorageAccessException((HttpStatusCode) result.HttpStatusCode);
            }
            catch (StorageException sex)
            {
                throw new StorageAccessException((HttpStatusCode)sex.RequestInformation.HttpStatusCode);
            }
        }

        public TE Get<TE>(string tableName, PartionKeyRowKeyPair paritionKeyRowKeyPair)
            where TE: TableEntity
        {
            var table = GetTableReference(tableName);

            try
            {
                var result =
                    table.Execute(TableOperation.Retrieve<TE>(paritionKeyRowKeyPair.PartitionKey,
                        paritionKeyRowKeyPair.RowKey));

                if (result.HttpStatusCode == (int) HttpStatusCode.OK && result.Result != null)
                    return (TE) result.Result;

                if (result.HttpStatusCode == (int) HttpStatusCode.NotFound ||
                    (result.HttpStatusCode == (int) HttpStatusCode.OK && result.Result == null))
                    return default(TE);

                throw new StorageAccessException((HttpStatusCode) result.HttpStatusCode);
            }
            catch (StorageException sex)
            {
                throw new StorageAccessException((HttpStatusCode)sex.RequestInformation.HttpStatusCode);
            }
        }

        public void Update<TE>(string tableName, TableEntity tableEntity)
            where TE : TableEntity
        {
            try
            {
                var result = GetTableReference(tableName).Execute(TableOperation.Replace(tableEntity));

                if (result.HttpStatusCode == (int) HttpStatusCode.PreconditionFailed)
                    throw new OptimisticConcurrencyException();

                if (result.HttpStatusCode != (int) HttpStatusCode.NoContent)
                    throw new StorageAccessException((HttpStatusCode) result.HttpStatusCode);
            }
            catch (StorageException sex)
            {
                throw new StorageAccessException((HttpStatusCode) sex.RequestInformation.HttpStatusCode);
            }
        }

        public void Upsert<TE>(string tableName, TableEntity tableEntity)
            where TE : TableEntity
        {
            try
            {
                var result = GetTableReference(tableName).Execute(TableOperation.InsertOrReplace(tableEntity));

                if (result.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                    throw new OptimisticConcurrencyException();

                if (result.HttpStatusCode != (int)HttpStatusCode.NoContent)
                    throw new StorageAccessException((HttpStatusCode)result.HttpStatusCode);
            }
            catch (StorageException sex)
            {
                throw new StorageAccessException((HttpStatusCode)sex.RequestInformation.HttpStatusCode);
            }
        }

        public void Delete(string tableName, TableEntity tableEntity)
        {
            try
            {
                var result = GetTableReference(tableName).Execute(TableOperation.Delete(tableEntity));

                if (result.HttpStatusCode == (int) HttpStatusCode.PreconditionFailed)
                    throw new OptimisticConcurrencyException();

                if (result.HttpStatusCode != (int) HttpStatusCode.NoContent)
                    throw new StorageAccessException((HttpStatusCode) result.HttpStatusCode);
            }
            catch (StorageException sex)
            {
                throw new StorageAccessException((HttpStatusCode) sex.RequestInformation.HttpStatusCode);
            }
        }

        public void EnsureTable(string tableName)
        {
            _tableClient.GetTableReference(tableName).CreateIfNotExists();
        }

        public IEnumerable<TE> QueryPartition<TE>(string tableName, string partitionKey) where TE : TableEntity, new()
        {
            var table = GetTableReference(tableName);

            var entities = new List<TE>();

            var rangeQuery = new TableQuery<TE>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            rangeQuery.TakeCount = 500;
            TableContinuationToken continuationToken = null;
            do
            {
                try
                {
                    var result = table.ExecuteQuerySegmented(rangeQuery, continuationToken);
                    entities.AddRange(result);
                    continuationToken = result.ContinuationToken;
                }
                catch (StorageException sex)
                {
                    throw new StorageAccessException((HttpStatusCode) sex.RequestInformation.HttpStatusCode);
                }

            } while (continuationToken != null);

            return entities;
        }

        protected CloudTable GetTableReference(string tableName)
        {
            return _tableClient.GetTableReference(tableName);
        }
    }
}
