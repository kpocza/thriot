using System.Configuration;

namespace IoT.Platform.Services.Telemetry.Configuration
{
    public class TelemetryDataSinkElement : ConfigurationElement
    {
        internal const string NameString = "name";
        internal const string TypeString = "type";
        internal const string DescriptionString = "description";
        internal const string ParameterPresetsString = "parameterPresets";

        [ConfigurationProperty(NameString, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this[NameString];
            }
            set
            {
                this[NameString] = value;
            }
        }

        [ConfigurationProperty(TypeString, IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this[TypeString];
            }
            set
            {
                this[TypeString] = value;
            }
        }

        [ConfigurationProperty(DescriptionString, IsRequired = false)]
        public string Description
        {
            get
            {
                return (string)this[DescriptionString];
            }
            set
            {
                this[DescriptionString] = value;
            }
        }

        [ConfigurationProperty(ParameterPresetsString, IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection), AddItemName = "param")]
        public KeyValueConfigurationCollection ParameterPresets
        {
            get { return (KeyValueConfigurationCollection)base[ParameterPresetsString]; }
            set { base[ParameterPresetsString] = value; }
        }
    }
}
