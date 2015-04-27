using Newtonsoft.Json.Linq;

namespace Thriot.Reporting.Dto
{
    public class CurrentDataDeviceDto
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }

        public long Timestamp { get; set; }

        public JToken Payload { get; set; }
    }
}
