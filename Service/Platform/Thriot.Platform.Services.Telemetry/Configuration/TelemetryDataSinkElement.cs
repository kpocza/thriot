using System;
using System.Xml.Serialization;

namespace Thriot.Platform.Services.Telemetry.Configuration
{
    [Serializable]
    public class TelemetryDataSinkElement
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlArray("parameterPresets")]
        [XmlArrayItem("param")]
        public TelemetrySinkParameter[] ParameterPresets { get; set; }
    }
}
