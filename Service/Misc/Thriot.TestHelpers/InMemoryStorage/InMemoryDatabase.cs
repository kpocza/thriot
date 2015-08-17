using System.Collections.Concurrent;

namespace Thriot.TestHelpers.InMemoryStorage
{
    public class InMemoryDatabase
    {
        private readonly ConcurrentDictionary<string, InMemoryTable> _tables;
        
        public static readonly InMemoryDatabase Instance = new InMemoryDatabase();

        private InMemoryDatabase()
        {
            _tables = new ConcurrentDictionary<string, InMemoryTable>();
        }

        public InMemoryTable Get(string tableName)
        {
            return _tables.GetOrAdd(tableName, new InMemoryTable());
        }
    }
}
