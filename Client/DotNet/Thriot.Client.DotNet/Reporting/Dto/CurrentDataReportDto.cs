using System.Collections.Generic;

namespace Thriot.Client.DotNet.Reporting
{
    /// <summary>
    /// Class that encapsultes one or more device's latest telemetry data
    /// </summary>
    public class CurrentDataReportDto
    {
        /// <summary>
        /// List of latest telemetry data
        /// </summary>
        public List<CurrentDataDeviceDto> Devices { get; set; }
    }
}
