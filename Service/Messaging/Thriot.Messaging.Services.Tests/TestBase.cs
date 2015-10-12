using System;
using Thriot.Messaging.Services.Caching;
using Thriot.Messaging.Services.Storage;
using Thriot.TestHelpers;

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
            var environmentFactoryFactory = EnvironmentFactoryFactory.Create();
            var connectionStringResolverTypeString =
                environmentFactoryFactory.MessagingEnvironment.ConnectionStringResolverType;

            object[] args = null;
            if (connectionStringResolverTypeString != null)
            {
                var connectionStringResolver = (IConnectionStringResolver)Activator.CreateInstance(Type.GetType(connectionStringResolverTypeString));
                args = new object[] {connectionStringResolver};
            }

            var persistentStorageTypeString =
                environmentFactoryFactory.MessagingEnvironment.PersistentStorageType;
            var persistentStorage = (IPersistentStorage)Activator.CreateInstance(Type.GetType(persistentStorageTypeString), args);

            return persistentStorage;
        }

        protected void RemoveCacheItem(long deviceId, int index)
        {
            _messageCache.Remove(new[] {new CacheIndex(deviceId, index)});
        }
    }
}
