using IoT.Objects.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Objects.Operations.Sql.Tests
{
    [TestClass]
    public class CompanyOperationsTest : CompanyOperationsTestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            Initialize();
        }

        [TestMethod]
        public override void GetCompanyTest()
        {
            base.GetCompanyTest();
        }
    }
}

