using System;
using Microsoft.ServiceBus.Messaging;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class EventhubReceiveAdapter : EventBasedQueueReceiveAdapter
    {
        private EventProcessorHost _eventProcessorHost;

        protected override void SetupEventProcessor()
        {
            var eventProcessorHostName = Guid.NewGuid().ToString();

            var eventHubConnectionString = _parameters["ConnectionString"];
            var eventHubName = _parameters["EventHubName"];
            var storageConnectionString = _parameters["StorageConnectionString"];

            _eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
        }

        protected override void InitializeEventProcessor()
        {
            _eventProcessorHost.RegisterEventProcessorFactoryAsync(new EventProcessorFactory(_receivedAction)).Wait();
        }

        protected override void StopEventProcessor()
        {
            _eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
