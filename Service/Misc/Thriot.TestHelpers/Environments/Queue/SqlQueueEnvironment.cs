using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Queue
{
    public class SqlQueueEnvironment : IQueueEnvironment
    {
        public string QueueConnectionString => @"Server=.\SQLEXPRESS;Database=ThriotTelemetryQueue;Trusted_Connection=True;";

        public IQueueSendAdapter QueueSendAdapter
        {
            get
            {
                var queueSendAdapter = InstanceCreator.Create<IQueueSendAdapter>("Thriot.Plugins.Sql.QueueSendAdapter, Thriot.Plugins.Sql");
                queueSendAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString } });
                return queueSendAdapter;
            }
        }

        public IQueueReceiveAdapter QueueReceiveAdapter
        {
            get
            {
                var queueReceiveAdapter = InstanceCreator.Create<IQueueReceiveAdapter>("Thriot.Plugins.Sql.QueueReceiveAdapter, Thriot.Plugins.Sql");
                queueReceiveAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString } });
                return queueReceiveAdapter;
            }
        }
    }
}
