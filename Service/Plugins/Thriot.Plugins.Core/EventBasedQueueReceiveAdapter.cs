using System;
using System.Collections.Generic;

namespace Thriot.Plugins.Core
{
    public abstract class EventBasedQueueReceiveAdapter : IQueueReceiveAdapter
    {
        protected IDictionary<string, string> _parameters;
        protected Action<TelemetryData> _receivedAction;

        public void Setup(IDictionary<string, string> parameters)
        {
            _parameters = parameters;

            SetupEventProcessor();
        }

        public void Start(Action<TelemetryData> receivedAction)
        {
            _receivedAction = receivedAction;

            InitializeEventProcessor();
        }

        public void Stop()
        {
            StopEventProcessor();
        }

        protected abstract void SetupEventProcessor();
        protected abstract void InitializeEventProcessor();
        protected abstract void StopEventProcessor();
    }
}
