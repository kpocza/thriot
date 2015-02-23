using System.Collections.Generic;
using IoT.Framework.Azure.TableOperations;

namespace IoT.Framework.Azure.DataAccess
{
    public abstract class Repository<T>
        where T : PreparableTableEntity, new()
    {
        private readonly ITableEntityOperation _tableEntityOperation;

        protected abstract string TableName { get; }

        protected Repository(ITableEntityOperation tableEntityOperation)
        {
            _tableEntityOperation = tableEntityOperation;
        }

        public void Create(T tableEntity)
        {
            tableEntity.PrepareBeforeSave();
            _tableEntityOperation.Insert<T>(TableName, tableEntity);
        }

        public T Get(PartionKeyRowKeyPair paritionKeyRowKeyPair)
        {
            var entity = _tableEntityOperation.Get<T>(TableName, paritionKeyRowKeyPair);
            if (entity != null)
            {
                entity.PrepareAfterLoad();
            }
            return entity;
        }

        public void Update(T tableEntity)
        {
            tableEntity.PrepareBeforeSave();
            _tableEntityOperation.Update<T>(TableName, tableEntity);
        }

        public void Upsert(T tableEntity)
        {
            tableEntity.PrepareBeforeSave();
            _tableEntityOperation.Upsert<T>(TableName, tableEntity);
        }

        public void Delete(T tableEntity)
        {
            _tableEntityOperation.Delete(TableName, tableEntity);
        }

        public IEnumerable<T> QueryPartition(string partitionKey)
        {
            return _tableEntityOperation.QueryPartition<T>(TableName, partitionKey);
        }
    }
}
