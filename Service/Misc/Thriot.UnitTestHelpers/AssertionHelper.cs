using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Thriot.TestHelpers
{
    public static class AssertionHelper
    {
        public static void AssertThrows<E>(Action functionThatFails, string errorMessage) where E : Exception
        {
            try
            {
                functionThatFails();
                Assert.Fail("No exception was thrown.");
            }
            catch (E ex)
            {
                Assert.IsTrue(ex.GetType().FullName == typeof(E).FullName, string.Format("Thrown exception was not type of {0}.", typeof(E).FullName));
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Assert.AreEqual(errorMessage, ex.Message, true, "The error message was not the same as expected.");
                }
            }
            catch (Exception ex)
            {
                if (ex is AssertFailedException)
                {
                    throw ex;
                }
                Assert.Fail("The exception thrown was not the type expected.");
            }
        }

        public static void AssertThrows<E>(Action functionThatFails) where E : Exception
        {
            AssertThrows<E>(functionThatFails, string.Empty);
        }
    }
}
