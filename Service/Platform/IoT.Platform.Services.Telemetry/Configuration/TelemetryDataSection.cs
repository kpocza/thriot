using System.Configuration;

namespace IoT.Platform.Services.Telemetry.Configuration
{
    public class TelemetryDataSection : ConfigurationSection
    {
        internal const string IncomingString = "incoming";

        [ConfigurationProperty(IncomingString, IsDefaultCollection = false, IsRequired = true)]
        [ConfigurationCollection(typeof(TelemetryDataSinkCollection), AddItemName = "sink")]
        public TelemetryDataSinkCollection Incoming
        {
            get { return (TelemetryDataSinkCollection)base[IncomingString]; }
            set { base[IncomingString] = value; }
        }
    }
}
