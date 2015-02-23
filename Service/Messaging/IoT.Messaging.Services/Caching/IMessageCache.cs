using System.Collections.Generic;

namespace IoT.Messaging.Services.Caching
{
    public interface IMessageCache
    {
        void Put(IEnumerable<CacheItem> items);

        CacheGetResult Get(IEnumerable<CacheIndex> indices);

        void Remove(IEnumerable<CacheIndex> indices);
    }
}
