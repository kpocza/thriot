using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Thriot.Messaging.Services.Caching
{
    public class MessageCache : IMessageCache
    {
        public void Put(IEnumerable<CacheItem> items)
        {
            foreach (var item in items)
            {
                MemoryCache.Default.Add(item.CacheKey, item,
                    new CacheItemPolicy() {SlidingExpiration = TimeSpan.FromMinutes(60)});
            }
        }

        public CacheGetResult Get(IEnumerable<CacheIndex> indices)
        {
            var result = new CacheGetResult();

            foreach (var idx in indices)
            {
                var cacheItem = (CacheItem)MemoryCache.Default.Get(idx.CacheKey);
                if (cacheItem != null)
                {
                    result.CacheItems.Add(idx, cacheItem);
                }
                else
                {
                    result.MissingDevices.Add(idx.DeviceId);
                }
            }

            return result;
        }

        public void Remove(IEnumerable<CacheIndex> indices)
        {
            foreach (var idx in indices)
            {
                MemoryCache.Default.Remove(idx.CacheKey);
            }
        }
    }
}