using System;
using Thriot.Framework;

namespace Thriot.TestHelpers
{
    public class EmailHelper
    {
        public static string Generate()
        {
            return "kpocza+iottest_" + Identity.Next() + "@gmail.com";
        }
    }
}
