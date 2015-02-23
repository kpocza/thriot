using Newtonsoft.Json.Linq;

namespace IoT.Reporting.Dto
{
    public class TimeSeriesRowDto
    {
        public long Timestamp { get; set; }

        public JToken Payload { get; set; }
    }
}
