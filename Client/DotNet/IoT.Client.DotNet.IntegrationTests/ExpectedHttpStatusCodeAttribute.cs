using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Thriot.Client.DotNet.IntegrationTests
{
    public class ExpectedHttpStatusCodeAttribute : ExpectedExceptionBaseAttribute
    {
        private readonly HttpStatusCode _expectedHttpStatusCode;

        public ExpectedHttpStatusCodeAttribute(HttpStatusCode expectedHttpStatusCode)
        {
            _expectedHttpStatusCode = expectedHttpStatusCode;
        }

        protected override void Verify(Exception exception)
        {
            if (exception is WebException)
            {
                var receivedStatusCode = ((HttpWebResponse) ((WebException) exception).Response).StatusCode;
                if (receivedStatusCode != _expectedHttpStatusCode)
                    throw new Exception("Expected status code: " + _expectedHttpStatusCode + " received: " +
                                        receivedStatusCode);
            }
            else
            {
                throw new Exception("Not a WebException");
            }
        }
    }
}
