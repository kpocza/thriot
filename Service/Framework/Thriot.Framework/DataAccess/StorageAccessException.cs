using System;
using System.Net;

namespace Thriot.Framework.DataAccess
{
    public class StorageAccessException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; private set; }

        public StorageAccessException(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}