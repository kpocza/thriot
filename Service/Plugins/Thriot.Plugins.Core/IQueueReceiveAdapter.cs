using System;

namespace Thriot.Plugins.Core
{
    public interface IQueueReceiveAdapter
    {
        void StartReceiver(Action<TelemetryData> receivedAction);

        void StopReceiver();
    }
}
