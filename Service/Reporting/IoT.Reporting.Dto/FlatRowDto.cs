using System;
using System.Collections.Generic;

namespace IoT.Reporting.Dto
{
    public class FlatRowDto
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }

        public long Timestamp { get; set; }

        public List<FlatPair> Fields { get; set; }
    }
}
