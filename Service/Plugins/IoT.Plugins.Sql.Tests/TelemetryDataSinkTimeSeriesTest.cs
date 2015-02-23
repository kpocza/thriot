using IoT.Framework.DataAccess;
using IoT.Plugins.Core;
using IoT.Plugins.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Plugins.Sql.Tests
{
    [TestClass]
    public class TelemetryDataSinkTimeSeriesTest : TelemetryDataSinkTimeSeriesTestBase
    {
        [TestInitialize]
        public void Init()
        {
            InitializeDevice();
        }

        protected override ITelemetryDataSinkTimeSeries GetTelemetryDataSinkTimeSeries()
        {
            return new TelemetryDataSinkTimeSeries();
        }

        protected override string GetConnectionString()
        {
            return @"Server=.\SQLEXPRESS;Database=IoTTelemetry;Trusted_Connection=True;";
        }

        [TestMethod]
        public override void RecordTest()
        {
            base.RecordTest();
        }

        [TestMethod]
        public override void RecordTwoTest()
        {
            base.RecordTwoTest();
        }

        [TestMethod]
        [ExpectedException(typeof(StorageAccessException))]
        public override void TryRecordTwiceTest()
        {
            base.TryRecordTwiceTest();
        }

        [TestMethod]
        public override void QueryDeviceTimeSeries()
        {
            base.QueryDeviceTimeSeries();
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
