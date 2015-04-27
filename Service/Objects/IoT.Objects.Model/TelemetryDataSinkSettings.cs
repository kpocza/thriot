using System.Collections.Generic;

namespace Thriot.Objects.Model
{
    public class TelemetryDataSinkSettings
    {
        public IEnumerable<TelemetryDataSinkParameters> Incoming { get; set; }
    }
}
