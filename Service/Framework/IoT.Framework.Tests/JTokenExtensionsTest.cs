using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace IoT.Framework.Tests
{
    [TestClass]
    public class JTokenExtensionsTest
    {
        [TestMethod]
        public void EmptyFlatTest()
        {
            var jToken = JToken.FromObject(new Object());

            jToken.EnsureRecognizableFormat();

            Assert.AreEqual(0, jToken.Children().Count());
        }

        [TestMethod]
        public void ObjWithValidParamsFlatTest()
        {
            var jToken = JToken.FromObject(new
            {
                Int = 1,
                Long = 1234567890L,
                String = "string",
                DateTime = DateTime.UtcNow,
                Bool = true,
                Guid = Guid.NewGuid(),
                Float = 3.14
            });

            jToken.EnsureRecognizableFormat();

            Assert.AreEqual(7, jToken.Children().Count());
        }

        [TestMethod]
        public void ObjWithByteArrayFlatTest()
        {
            var jToken = JToken.FromObject(new
            {
                Int = 1,
                Long = 1234567890L,
                String = "string",
                DateTime = DateTime.UtcNow,
                Bool = true,
                Guid = Guid.NewGuid(),
                Float = 3.14,
                Bytes = new byte[] {1, 2, 3, 4, 5}
            });

            jToken.EnsureRecognizableFormat();

            Assert.AreEqual(7, jToken.Children().Count());
        }

        [TestMethod]
        public void ValidObjsNullParamFlatTest()
        {
            var jToken = JToken.FromObject(new
            {
                Int = 1,
                String = (string)null,
            });

            jToken.EnsureRecognizableFormat();

            Assert.AreEqual(1, jToken.Children().Count());
        }

        [TestMethod]
        public void ObjWithChildrenFlatTest()
        {
            var jToken = JToken.FromObject(new
            {
                Int = 1,
                ChildObj = new
                {
                    Something = 1
                }
            });

            jToken.EnsureRecognizableFormat();

            Assert.AreEqual(1, jToken.Children().Count());
        }
    }
}
