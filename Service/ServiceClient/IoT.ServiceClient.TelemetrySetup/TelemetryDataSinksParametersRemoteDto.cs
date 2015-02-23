using System.Collections.Generic;

namespace IoT.ServiceClient.TelemetrySetup
{
    public class TelemetryDataSinksParametersRemoteDto
    {
        public List<TelemetryDataSinkParametersRemoteDto> Incoming { get; set; } 
    }
}