using System.Configuration;

namespace Thriot.Client.DotNet.IntegrationTests
{
    public abstract class TestBase
    {
        protected static readonly string ManagementApi = ConfigurationManager.AppSettings["ManagementApi"];
        protected static readonly string PlatformApi = ConfigurationManager.AppSettings["PlatformApi"];
        protected static readonly string ReportingApi = ConfigurationManager.AppSettings["ReportingApi"];
        protected static readonly string PlatformWebSocketApi = ConfigurationManager.AppSettings["PlatformWebSocketApi"];
        protected static readonly string SinkData = ConfigurationManager.AppSettings["sinkData"];
        protected static readonly string SinkTimeSeries = ConfigurationManager.AppSettings["sinkTimeSeries"];
        protected static readonly string ParamSinkData = ConfigurationManager.AppSettings["paramSinkData"];
        protected static readonly string ParamSinkDataConnectionString = ConfigurationManager.AppSettings["paramSinkDataConnectionString"];
    }
}
