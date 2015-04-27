using System.Net.Http;
using System.Web.Http.Filters;

namespace Thriot.Framework.Web.ApiExceptions
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exceptionType = actionExecutedContext.Exception.GetType();

            var httpStatusCode = ApiExceptionRegistry.GetHttpStatusCode(exceptionType);

            var response = new HttpResponseMessage(httpStatusCode);
            response.Content = new StringContent(actionExecutedContext.Exception.Message);

            actionExecutedContext.Response = response;
        }
    }
}
