using System.Collections.Generic;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using Thriot.Framework;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class EventhubSendAdapter : IQueueSendAdapter
    {
        private EventHubClient _eventHubClient;

        public void Setup(IDictionary<string, string> parameters)
        {
            var connectionString = parameters["ConnectionString"];
            var eventHubName = parameters["EventHubName"];

            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
        }

        public void Send(TelemetryData telemetryData)
        {
            var content = Serializers.ToJsonString(telemetryData);
            var eventData = new EventData(Encoding.UTF8.GetBytes(content));
            _eventHubClient.Send(eventData);
        }

        public void Clear()
        {
            // not implemented
        }
    }
}
