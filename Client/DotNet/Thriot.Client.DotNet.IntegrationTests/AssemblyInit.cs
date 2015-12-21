using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Thriot.Client.DotNet.IntegrationTests
{
    [TestClass]
    public class AssemblyInit
    {
        [AssemblyInitialize]
        public static void AssemblyInitFunction(TestContext context)
        {
            TestBase.ManagementApi = (string)context.Properties["ManagementApi"];
            TestBase.PlatformApi = (string)context.Properties["PlatformApi"];
            TestBase.ReportingApi = (string)context.Properties["ReportingApi"];
            TestBase.PlatformWebsocketApi = (string)context.Properties["PlatformWebsocketApi"];
            TestBase.SinkData = (string)context.Properties["sinkData"];
            TestBase.SinkTimeSeries = (string)context.Properties["sinkTimeSeries"];
            TestBase.ParamSinkData = (string)context.Properties["paramSinkData"];
            TestBase.ParamSinkDataConnectionString = (string)context.Properties["paramSinkDataConnectionString"];
            TestBase.ParamSinkDataKeyspace = (string)context.Properties["paramSinkDataKeyspace"];
            TestBase.ParamSinkDataContactPoints = (string)context.Properties["paramSinkDataContactPoints"];
        }
    }
}
