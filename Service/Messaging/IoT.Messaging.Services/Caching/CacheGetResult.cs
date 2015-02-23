using System.Collections.Generic;

namespace IoT.Messaging.Services.Caching
{
    public class CacheGetResult
    {
        public CacheGetResult()
        {
            CacheItems = new Dictionary<CacheIndex, CacheItem>();
            MissingDevices = new HashSet<long>();
        }

        public IDictionary<CacheIndex, CacheItem> CacheItems { get; private set; }

        public ICollection<long> MissingDevices { get; private set; }
    }
}