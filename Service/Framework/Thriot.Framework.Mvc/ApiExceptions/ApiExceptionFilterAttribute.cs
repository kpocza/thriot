using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace Thriot.Framework.Mvc.ApiExceptions
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext exceptionContext)
        {
            var httpStatusCode = ApiExceptionRegistry.GetHttpStatusCode(exceptionContext.Exception.GetType());
            var response = new ContentResult
            {
                Content = exceptionContext.Exception.Message,
                StatusCode = (int)httpStatusCode
            };

            exceptionContext.Result = response;
        }
    }
}
