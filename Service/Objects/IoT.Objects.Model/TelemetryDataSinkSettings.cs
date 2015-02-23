using System.Collections.Generic;

namespace IoT.Objects.Model
{
    public class TelemetryDataSinkSettings
    {
        public IEnumerable<TelemetryDataSinkParameters> Incoming { get; set; }
    }
}
