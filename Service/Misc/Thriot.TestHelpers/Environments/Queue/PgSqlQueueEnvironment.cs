using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.Environments.Queue
{
    public class PgSqlQueueEnvironment : IQueueEnvironment
    {
        public string QueueConnectionString => "Server=127.0.0.1;Port=5432;Database=ThriotTelemetryQueue;User Id=thriottelemetryqueue;Password=thriottelemetryqueue;";

        public IQueueSendAdapter QueueSendAdapter
        {
            get
            {
                var queueSendAdapter = InstanceCreator.Create<IQueueSendAdapter>("Thriot.Plugins.PgSql.QueueSendAdapter, Thriot.Plugins.PgSql");
                queueSendAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString } });
                return queueSendAdapter;
            }
        }

        public IQueueReceiveAdapter QueueReceiveAdapter
        {
            get
            {
                var queueReceiveAdapter = InstanceCreator.Create<IQueueReceiveAdapter>("Thriot.Plugins.PgSql.QueueReceiveAdapter, Thriot.Plugins.PgSql");
                queueReceiveAdapter.Setup(new Dictionary<string, string> { { "ConnectionString", QueueConnectionString } });
                return queueReceiveAdapter;
            }
        }
    }
}
