using System;
using Microsoft.ServiceBus.Messaging;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class EventProcessorFactory : IEventProcessorFactory
    {
        private readonly Action<TelemetryData> _receivedAction;

        public EventProcessorFactory(Action<TelemetryData> receivedAction)
        {
            _receivedAction = receivedAction;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor(_receivedAction);
        }
    }
}