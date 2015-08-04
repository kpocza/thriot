using System;
using System.Xml.Serialization;

namespace Thriot.Platform.Services.Telemetry.Configuration
{
    [Serializable]
    [XmlRoot("telemetryDataSink")]
    public class TelemetryDataSection
    {
        [XmlArray("incoming")]
        [XmlArrayItem("sink")]
        public TelemetryDataSinkElement[] Incoming { get; set; }
    }
}
