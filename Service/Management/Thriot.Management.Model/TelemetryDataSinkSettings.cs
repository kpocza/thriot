using System.Collections.Generic;

namespace Thriot.Management.Model
{
    public class TelemetryDataSinkSettings
    {
        public IEnumerable<TelemetryDataSinkParameters> Incoming { get; set; }
    }
}
