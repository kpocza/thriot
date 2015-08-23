using System.Collections.Generic;

namespace Thriot.Plugins.Core
{
    public interface IQueueSendAdapter
    {
        void Setup(IDictionary<string, string> parameters);

        void Send(TelemetryData telemetryData);

        void Clear();
    }
}
