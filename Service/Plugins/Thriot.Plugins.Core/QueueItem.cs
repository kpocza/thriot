namespace Thriot.Plugins.Core
{
    public class QueueItem
    {
        public long Id { get; private set; }
        public object NativeObject { get; private set; }
        public TelemetryData TelemetryData { get; private set; }

        public QueueItem(long id, TelemetryData telemetryData)
        {
            Id = id;
            TelemetryData = telemetryData;
        }

        public QueueItem(object nativeObject, TelemetryData telemetryData)
        {
            NativeObject = nativeObject;
            TelemetryData = telemetryData;
        }
    }
}
