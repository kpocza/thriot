using System.Collections.Generic;
using Thriot.Plugins.Core;

namespace Thriot.TestHelpers.InMemoryQueue
{
    public class InMemoryQueueSendAdapter : IQueueSendAdapter
    {
        public void Setup(IDictionary<string, string> parameters)
        {
        }

        public void Send(TelemetryData telemetryData)
        {
            InMemoryQueue.Instance.Enqueue(telemetryData);
        }

        public void Clear()
        {
            InMemoryQueue.Instance.Clear();
        }
    }
}
