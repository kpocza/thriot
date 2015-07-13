using System.Collections.Generic;
using Thriot.Framework.DataAccess;

namespace Thriot.Plugins.Core
{
    public interface ITelemetryDataSink
    {
        void Setup(IDynamicConnectionStringResolver dynamicConnectionStringResolver, IDictionary<string, string> parameters);

        void Initialize();

        void Record(TelemetryData message);

        IReadOnlyCollection<string> ParametersNames { get; }

        IDictionary<string, string> ParameterSubstitutes { get; }
    }
}