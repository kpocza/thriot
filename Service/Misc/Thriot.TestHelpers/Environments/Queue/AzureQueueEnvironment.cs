using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Queue
{
    public class AzureQueueEnvironment : IQueueEnvironment
    {
        public string QueueConnectionString => "UseDevelopmentStorage=true";

        public IQueueSendAdapter QueueSendAdapter
        {
            get
            {
                var queueSendAdapter = InstanceCreator.Create<IQueueSendAdapter>("Thriot.Plugins.Azure.QueueSendAdapter, Thriot.Plugins.Azure");
                queueSendAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString }, { "QueueName", "telemetry" } });
                return queueSendAdapter;
            }
        }

        public IQueueReceiveAdapter QueueReceiveAdapter
        {
            get
            {
                var queueReceiveAdapter = InstanceCreator.Create<IQueueReceiveAdapter>("Thriot.Plugins.Azure.QueueReceiveAdapter, Thriot.Plugins.Azure");
                queueReceiveAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString }, { "QueueName", "telemetry" } });
                return queueReceiveAdapter;
            }
        }
    }
}
