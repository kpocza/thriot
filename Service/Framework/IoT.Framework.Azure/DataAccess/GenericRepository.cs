using IoT.Framework.Azure.TableOperations;

namespace IoT.Framework.Azure.DataAccess
{
    public class GenericRepository<T> : Repository<T>
        where T : PreparableTableEntity, new()
    {
        private readonly string _tableName;

        public GenericRepository(ITableEntityOperation tableEntityOperation, string tableName)
            : base(tableEntityOperation)
        {
            _tableName = tableName;
        }

        protected override string TableName
        {
            get { return _tableName; }
        }
    }
}
