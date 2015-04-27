using Thriot.Framework;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;

namespace Thriot.Messaging.Services.Tests
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
