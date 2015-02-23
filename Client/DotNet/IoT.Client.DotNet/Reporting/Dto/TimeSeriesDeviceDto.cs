using System.Collections.Generic;

namespace IoT.Client.DotNet.Reporting
{
    public class TimeSeriesDeviceDto
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }

        public List<TimeSeriesRowDto> Data { get; set; }
    }
}
