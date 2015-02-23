using System.Collections.Generic;

namespace IoT.Reporting.Dto
{
    public class TimeSeriesDeviceDto
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }

        public List<TimeSeriesRowDto> Data { get; set; }
    }
}
