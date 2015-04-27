using System.Collections.Generic;

namespace Thriot.Client.DotNet.Reporting
{
    /// <summary>
    /// Time series data for a device
    /// </summary>
    public class TimeSeriesDeviceDto
    {
        /// <summary>
        /// Unique device id
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Name of the device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of historical time series data
        /// </summary>
        public List<TimeSeriesRowDto> Data { get; set; }
    }
}
