using Thriot.Plugins.Core;
using Thriot.TestHelpers.InMemoryQueue;

namespace Thriot.TestHelpers.Environments.Queue
{
    public class InMemoryQueueEnvironment : IQueueEnvironment
    {
        public IQueueSendAdapter QueueSendAdapter => new InMemoryQueueSendAdapter();

        public IQueueReceiveAdapter QueueReceiveAdapter => new InMemorySerialQueueReceiveAdapter();
    }
}
