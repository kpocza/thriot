using Newtonsoft.Json.Linq;

namespace Thriot.Reporting.Services.Dto
{
    public class TimeSeriesRowDto
    {
        public long Timestamp { get; set; }

        public JToken Payload { get; set; }
    }
}
