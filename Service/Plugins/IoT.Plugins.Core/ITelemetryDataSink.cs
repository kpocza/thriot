using System.Collections.Generic;

namespace IoT.Plugins.Core
{
    public interface ITelemetryDataSink
    {
        void Setup(IDictionary<string, string> parameters);

        void Initialize();

        void Record(TelemetryData message);

        IReadOnlyCollection<string> ParametersNames { get; }

        IDictionary<string, string> ParameterSubstitutes { get; }
    }
}