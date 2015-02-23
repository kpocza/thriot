using System;
using System.Collections.Generic;

namespace IoT.Plugins.Core
{
    public class TelemetryData
    {
        public TelemetryData(string deviceId, string payload, DateTime time)
        {
            DeviceId = deviceId;
            Payload = payload;
            Time = time;
        }

        public string DeviceId { get; private set; }

        public string Payload { get; private set; }

        public DateTime Time { get; private set; }
    }
}
