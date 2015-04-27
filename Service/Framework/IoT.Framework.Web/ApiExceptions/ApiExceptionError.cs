using System.Net;

namespace Thriot.Framework.Web.ApiExceptions
{
    public class ApiExceptionError
    {
        public ApiExceptionError(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; private set; }
    }
}
