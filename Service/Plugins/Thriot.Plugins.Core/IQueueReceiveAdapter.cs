using System;
using System.Collections.Generic;

namespace Thriot.Plugins.Core
{
    public interface IQueueReceiveAdapter
    {
        void Setup(IDictionary<string, string> parameters);

        void Start(Action<TelemetryData> receivedAction);

        void Stop();
    }
}
