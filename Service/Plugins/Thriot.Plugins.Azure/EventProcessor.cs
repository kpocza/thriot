using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Thriot.Framework;
using Thriot.Plugins.Core;

namespace Thriot.Plugins.Azure
{
    public class EventProcessor : IEventProcessor
    {
        private Stopwatch _checkpointStopWatch;
        private readonly Action<TelemetryData> _receivedAction;

        public EventProcessor(Action<TelemetryData> receivedAction)
        {
            _receivedAction = receivedAction;
        }

        public Task OpenAsync(PartitionContext context)
        {
            _checkpointStopWatch = new Stopwatch();
            _checkpointStopWatch.Start();

            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                    
                var telemetryData = Serializers.FromJsonString<TelemetryData>(data);
                _receivedAction(telemetryData);
            }

            if (_checkpointStopWatch.Elapsed > TimeSpan.FromSeconds(30) || messages.Count() > 100)
            {
                context.CheckpointAsync().Wait();
                _checkpointStopWatch.Restart();
            }

            return Task.FromResult<object>(null);
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                context.CheckpointAsync().Wait();
            }

            return Task.FromResult<object>(null);
        }
    }
}