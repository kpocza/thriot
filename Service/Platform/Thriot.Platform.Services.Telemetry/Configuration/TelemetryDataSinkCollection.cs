using System.Configuration;

namespace Thriot.Platform.Services.Telemetry.Configuration
{
    public class TelemetryDataSinkCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TelemetryDataSinkElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as TelemetryDataSinkElement).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public void Add(ConfigurationElement element)
        {
            base.BaseAdd(element);
        }

        protected override string ElementName
        {
            get { return "sink"; }
        }
    }
}
