using Thriot.Plugins.Core;

namespace Thriot.TestHelpers
{
    public interface IQueueEnvironment
    {
        IQueueSendAdapter QueueSendAdapter { get; }

        IQueueReceiveAdapter QueueReceiveAdapter { get; }
    }
}
