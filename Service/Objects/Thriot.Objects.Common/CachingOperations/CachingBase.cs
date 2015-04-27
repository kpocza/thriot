using System;
using System.Runtime.Caching;

namespace Thriot.Objects.Common.CachingOperations
{
    public abstract class CachingBase<T>
        where T: class
    {
        protected abstract string Prefix { get; }

        protected T Get(object id, Func<object, object> _loadFromStorage)
        {
            var cacheKey = Prefix + "_" + id.ToString();

            var value = (T)MemoryCache.Default.Get(cacheKey);
            if (value != null)
                return value;

            value = (T)_loadFromStorage(id);

            MemoryCache.Default.Set(cacheKey, value, DateTime.UtcNow.AddMinutes(10));

            return value;
        }

        public void Remove(object id)
        {
            var cacheKey = Prefix + "_" + id.ToString();

            MemoryCache.Default.Remove(cacheKey);
        }
    }
}
