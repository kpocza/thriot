using Newtonsoft.Json.Linq;

namespace IoT.Platform.PersistentConnections.Commands
{
    public class TelemetryDataCommand : Command
    {
        public JToken TelemetryData { get; private set; }

        public TelemetryDataCommand(string parameter)
        {
            try
            {
                TelemetryData = JToken.Parse(parameter);
            }
            catch
            {
                return;
            }

            IsValid = true;
        }
    }
}
