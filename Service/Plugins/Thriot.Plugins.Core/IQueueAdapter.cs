using System;

namespace Thriot.Plugins.Core
{
    public interface IQueueAdapter
    {
        void Send(TelemetryData telemetryData);

        void RegisterReceiveCallback(Action<TelemetryData> receivedAction);

        void StartReceiver();

        void StopReceiver();
    }
}
