using System;
using System.Configuration;
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
            var persistentStorage = PoorMansContainerResolver();
            _messageCache = new MessageCache();
            MessagingService = new MessagingService(_messageCache, persistentStorage);
        }

        private static IPersistentStorage PoorMansContainerResolver()
        {
            var connectionStringResolverTypeString = ConfigurationManager.AppSettings["IConnectionStringResolver"];
            object[] args = null;
            if (connectionStringResolverTypeString != null)
            {
                var connectionStringResolver = (IConnectionStringResolver)Activator.CreateInstance(Type.GetType(connectionStringResolverTypeString));
                args = new object[] {connectionStringResolver};
            }

            var persistentStorage = (IPersistentStorage)Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["IPersistentStorage"]), args);

            return persistentStorage;
        }

        protected void RemoveCacheItem(long deviceId, int index)
        {
            _messageCache.Remove(new[] {new CacheIndex(deviceId, index)});
        }
    }
}
