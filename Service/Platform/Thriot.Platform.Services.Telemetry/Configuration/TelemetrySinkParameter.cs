using System;
using System.Xml.Serialization;

namespace Thriot.Platform.Services.Telemetry.Configuration
{
    [Serializable]
    public class TelemetrySinkParameter
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
