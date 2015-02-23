using IoT.Plugins.Core;
using IoT.Plugins.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Plugins.Azure.Tests
{
    [TestClass]
    public class TelemetryDataSinkCurrentTest : TelemetryDataSinkCurrentTestBase
    {
        [TestInitialize]
        public void Init()
        {
            InitializeDevice();
        }

        protected override ITelemetryDataSinkCurrent GetTelemetryDataSinkCurrent()
        {
            return new TelemetryDataSinkCurrent();
        }

        protected override string GetConnectionString()
        {
            return "UseDevelopmentStorage=true";
        }

        [TestMethod]
        public override void RecordTest()
        {
            base.RecordTest();
        }

        [TestMethod]
        public override void RecordTwiceTest()
        {
            base.RecordTwiceTest();
        }

        [TestMethod]
        public override void QueryDeviceCurrentData()
        {
            base.QueryDeviceCurrentData();
        }

        protected override bool IsIntegrationTest()
        {
            {
#if INTEGRATIONTEST
            return true;
#endif
                return false;
            }
        }
    }
}
