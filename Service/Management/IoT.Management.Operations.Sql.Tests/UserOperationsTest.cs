using IoT.Framework.Exceptions;
using IoT.Management.Operations.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoT.Management.Operations.Sql.Tests
{
    [TestClass]
    public class UserOperationsTest : UserOperationsTestBase
    {
        [TestMethod]
        public override void CreateTest()
        {
            base.CreateTest();
        }

        [TestMethod]
        public override void IsNotExistsTest()
        {
            base.IsNotExistsTest();
        }

        [TestMethod]
        public override void IsExistsTest()
        {
            base.IsExistsTest();
        }

        [TestMethod]
        public override void GetMeSuccessTest()
        {
            base.GetMeSuccessTest();
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public override void GetMeFailedTest()
        {
            base.GetMeFailedTest();
        }

        [TestMethod]
        public override void UpdateSuccessTest()
        {
            base.UpdateSuccessTest();
        }
    }
}
