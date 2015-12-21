using System.Configuration;

namespace Thriot.Client.DotNet.IntegrationTests
{
    public abstract class TestBase
    {
        internal static string ManagementApi { get; set; }
        internal static string PlatformApi { get; set; }
        internal static string ReportingApi { get; set; }
        internal static string PlatformWebsocketApi { get; set; }
        internal static string SinkData { get; set; }
        internal static string SinkTimeSeries { get; set; }
        internal static string ParamSinkData { get; set; }
        internal static string ParamSinkDataConnectionString { get; set; }
        internal static string ParamSinkDataKeyspace { get; set; }
        internal static string ParamSinkDataContactPoints { get; set; }
    }
}
