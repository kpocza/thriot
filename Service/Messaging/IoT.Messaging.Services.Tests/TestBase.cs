using IoT.Framework;
using IoT.Messaging.Services.Caching;
using IoT.Messaging.Services.Storage;

namespace IoT.Messaging.Services.Tests
{
    public class TestBase
    {
        protected MessagingService MessagingService;
        private MessageCache _messageCache;

        public void Initialize()
        {
            var persistentStorage = SingleContainer.Instance.Resolve<IPersistentStorage>();
            _messageCache = new MessageCache();
            MessagingService = new MessagingService(_messageCache, persistentStorage);
        }

        protected void RemoveCacheItem(long deviceId, int index)
        {
            _messageCache.Remove(new[] {new CacheIndex(deviceId, index)});
        }
    }
}
