namespace Thriot.Plugins.Core
{
    public interface IQueueSendAdapter
    {
        void Send(TelemetryData telemetryData);
    }
}
