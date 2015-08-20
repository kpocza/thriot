using System;

namespace Thriot.Plugins.Core
{
    public interface IQueueReceiveAdapter
    {
        void Start(Action<TelemetryData> receivedAction);

        void Stop();
    }
}
