﻿using System.Collections.Generic;

namespace Thriot.Management.Model
{
    public class TelemetryDataSinkParameters
    {
        public string SinkName { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
