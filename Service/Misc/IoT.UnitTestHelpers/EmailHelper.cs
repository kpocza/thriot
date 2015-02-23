using System;
using IoT.Framework;

namespace IoT.UnitTestHelpers
{
    public class EmailHelper
    {
        public static string Generate()
        {
            return "kpocza+iottest_" + Identity.Next() + "@gmail.com";
        }
    }
}
