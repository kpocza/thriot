using System.Collections.Generic;

namespace IoT.Client.DotNet.Reporting
{
    /// <summary>
    /// Class that encapsulates one of more device's time series data
    /// </summary>
    public class TimeSeriesReportDto
    {
        /// <summary>
        /// List of one or mode device's time series data
        /// </summary>
        public List<TimeSeriesDeviceDto> Devices { get; set; }
    }
}
